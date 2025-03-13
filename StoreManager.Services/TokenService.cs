using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using StoreManager.DTO;
using StoreManager.Extensions;
using StoreManager.Facade.Interfaces.Repositories;
using StoreManager.Facade.Interfaces.Services;
using StoreManager.Facade.Interfaces.Utilities;
using StoreManager.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace StoreManager.Services
{
    public class TokenService : ITokenService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRequestHelper _userRequestHelper;
        private readonly ILogger<TokenService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly byte[] _key;

        private DateTime AccessTokenExpieresTime => DateTime.UtcNow.AddMinutes(30);
        private DateTime RefreshTokenExpieresTime => DateTime.UtcNow.AddDays(30);

        public TokenService(IUnitOfWork unitOfWork,
            IUserRequestHelper userRequestHelper,
            ILogger<TokenService> logger,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _userRequestHelper = userRequestHelper ?? throw new ArgumentNullException(nameof(userRequestHelper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));

            _issuer = _configuration["Jwt:Issuer"]!;
            _audience = _configuration["Jwt:Audience"]!;
            _key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]!.PadRight(64, '0'));
        }

        public async Task<int> InsertAsync(Token token)
        {
            try
            {
                _logger.LogInformation("🔹 Inserting new token for AccountId: {AccountId}", token.AccountId);
                int tokenId = (int)await _unitOfWork.TokenRepository.InsertAsync(token);
                _logger.LogInformation("✅ Token inserted successfully. TokenId: {TokenId}, AccountId: {AccountId}", tokenId, token.AccountId);
                return tokenId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error inserting token for AccountId: {AccountId}", token.AccountId);
                throw new InvalidOperationException("An error occurred while inserting the token.", ex);
            }
        }

        public async Task<bool> IsTokenValidAsync(string tokenString)
        {
            try
            {
                _logger.LogInformation("🔍 Validating token...");
                bool isValid = await _unitOfWork.TokenRepository.IsTokenValidAsync(tokenString);

                if (isValid)
                {
                    _logger.LogInformation("✅ Token is valid.");
                }
                else
                {
                    _logger.LogWarning("⚠️ Token is invalid or expired.");
                }

                return isValid;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error validating token.");
                throw new InvalidOperationException("An error occurred while validating the token.", ex);
            }
        }

        public async Task RevokeTokenAsync(string tokenString)
        {
            try
            {
                _logger.LogInformation("🚫 Revoking token...");
                await _unitOfWork.TokenRepository.RevokeTokenAsync(tokenString);
                _logger.LogInformation("✅ Token revoked successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error revoking token.");
                throw new InvalidOperationException("An error occurred while revoking the token.", ex);
            }
        }

        public async Task<string> RefreshToken(string refreshToken)
        {
            await _unitOfWork.OpenConnectionAsync();
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var token = await _unitOfWork.TokenRepository.GetByRefreshToken(refreshToken);
                if (token == null)
                {
                    _logger.LogWarning("🔑 Invalid refresh token used.");
                    throw new UnauthorizedAccessException("Invalid refresh token.");
                }

                if (token.RevokedAt != null)
                {
                    _logger.LogWarning("⚠️ Refresh token has already been revoked.");
                    throw new UnauthorizedAccessException("Refresh token has been revoked.");
                }

                if (token.RefreshTokenExpiresAt <= DateTime.UtcNow)
                {
                    _logger.LogWarning("⚠️ Refresh token has expired.");
                    throw new UnauthorizedAccessException("Refresh token has expired.");
                }

                var account = await _unitOfWork.AccountRepository.GetByIdAsync(token.AccountId);
                if (account == null)
                {
                    _logger.LogWarning("❌ Account associated with refresh token not found.");
                    throw new UnauthorizedAccessException("Account not found.");
                }

                var ipAddress = _userRequestHelper.GetUserIpAddress();
                var deviceInfo = _userRequestHelper.GetDeviceDetails();

                if (token.IpAddress != ipAddress)
                {
                    _logger.LogWarning("🚨 Suspicious activity detected: Refresh token used from a new IP address. Old: {OldIP}, New: {NewIP}", token.IpAddress, ipAddress);
                    throw new SecurityException("Suspicious activity detected. Refresh token used from a new IP address.");
                }

                token.RevokedAt = DateTime.UtcNow;
                await _unitOfWork.TokenRepository.UpdateAsync(token);

                var newAccessToken = CreateJwtToken(account);
                var newRefreshToken = GenerateRefreshToken();

                var newToken = new Token
                {
                    AccountId = account.Id,
                    AccessTokenHash = newAccessToken.HashToken(),
                    RefreshToken = newRefreshToken,
                    AccessTokenExpiresAt = AccessTokenExpieresTime,
                    RefreshTokenExpiresAt = RefreshTokenExpieresTime,
                    DeviceInfo = deviceInfo,
                    IpAddress = ipAddress,
                    CreateDate = DateTime.UtcNow,
                    IsActive = true
                };

                await _unitOfWork.TokenRepository.InsertAsync(newToken);

                SetJwtCookie(newAccessToken);
                SetRefreshTokenCookie(newRefreshToken);

                await _unitOfWork.CommitAsync();

                _logger.LogInformation("✅ Refresh token successfully processed for AccountId: {AccountId}, IP: {IpAddress}, Device: {DeviceInfo}", account.Id, ipAddress, deviceInfo);

                return newAccessToken;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error during refresh token process.");
                await _unitOfWork.RollBackAsync();
                throw new UnauthorizedAccessException("An error occurred while refreshing the token.");
            }
            finally
            {
                await _unitOfWork.CloseConnectionAsync();
            }
        }

        public TokenResponse GenerateTokenAsync(Account account)
        {
            var accessToken = CreateJwtToken(account);
            var refreshToken = GenerateRefreshToken();

            SetJwtCookie(accessToken);
            SetRefreshTokenCookie(refreshToken);

            return new TokenResponse
            {
                AccessToken = accessToken,
                AccountId = account.Id,
                RefreshToken = refreshToken,
                AccessTokenExpiresAt = AccessTokenExpieresTime,
                RefreshTokenExpiresAt = RefreshTokenExpieresTime
            };
        }

        private string CreateJwtToken(Account account)
        {
            var tokenExpiration = AccessTokenExpieresTime;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()),
                new Claim(ClaimTypes.Email, account.Email)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = tokenExpiration,
                Issuer = _issuer,
                Audience = _audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(_key), SecurityAlgorithms.HmacSha512Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private void SetJwtCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            };

            _httpContextAccessor.HttpContext.Response.Cookies.Append("jwtToken", token, cookieOptions);
        }
        private void SetRefreshTokenCookie(string refreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = RefreshTokenExpieresTime
            };

            _httpContextAccessor.HttpContext.Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }

        private string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            return Convert.ToBase64String(randomBytes);
        }
    }
}
