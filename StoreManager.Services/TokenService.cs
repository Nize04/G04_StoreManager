using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using StoreManager.DTO;
using StoreManager.Extensions;
using StoreManager.Facade.Interfaces.Repositories;
using StoreManager.Facade.Interfaces.Services;
using StoreManager.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace StoreManager.Services
{
    public class TokenService : ITokenService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly byte[] _key;

        private DateTime AccessTokenExpieresTime => DateTime.UtcNow.AddMinutes(30);
        private DateTime RefreshTokenExpieresTime => DateTime.UtcNow.AddDays(30);

        public TokenService(IUnitOfWork unitOfWork, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));

            _issuer = _configuration["Jwt:Issuer"]!;
            _audience = _configuration["Jwt:Audience"]!;
            _key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]!.PadRight(64, '0'));
        }

        public async Task<int> InsertAsync(Token token)
        {
            await _unitOfWork.OpenConnectionAsync();
            try
            {
                return (int)await _unitOfWork.TokenRepository.InsertAsync(token);
            }
            finally
            {
                await _unitOfWork.CloseConnectionAsync();
            }
        }

        public async Task<bool> IsTokenValidAsync(string tokenString)
        {
            await _unitOfWork.OpenConnectionAsync();
            try
            {
                return await _unitOfWork.TokenRepository.IsTokenValidAsync(tokenString);
            }
            finally
            {
                await _unitOfWork.CloseConnectionAsync();
            }
        }

        public async Task RevokeTokenAsync(string tokenString)
        {
            await _unitOfWork.OpenConnectionAsync();
            try
            {
                await _unitOfWork.TokenRepository.RevokeTokenAsync(tokenString);
            }
            finally
            {
                await _unitOfWork.CloseConnectionAsync();
            }
        }

       
       public async Task<string> RefreshToken(string refreshToken)
       {
           await _unitOfWork.OpenConnectionAsync();

           await _unitOfWork.BeginTransactionAsync();

           try
           {
               var token = await _unitOfWork.TokenRepository.GetByRefreshToken(refreshToken);
               if (token == null || token.RevokedAt >= DateTime.UtcNow || token.RefreshTokenExpiresAt <= DateTime.UtcNow)
               {
                   throw new UnauthorizedAccessException("Invalid or expired refresh token.");
               }

               var account = await _unitOfWork.AccountRepository.GetByIdAsync(token.AccountId);
               if (account == null) throw new UnauthorizedAccessException("Account not found.");

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
                   DeviceInfo = UserRequestHelper.GetDeviceDetails()
               };

               await _unitOfWork.TokenRepository.InsertAsync(newToken);

               SetJwtCookie(newAccessToken);
               SetRefreshTokenCookie(newRefreshToken);
               
               await _unitOfWork.CommitAsync();

               return newAccessToken;
           }
           catch
           {
               await _unitOfWork.RollBackAsync();
               throw;
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
