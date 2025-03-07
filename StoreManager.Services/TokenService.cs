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
        private DateTime AccessTokenExpieresTime => DateTime.UtcNow.AddMinutes(30);
        private DateTime RefreshTokenExpieresTime => DateTime.UtcNow.AddDays(30);

        public TokenService(IUnitOfWork unitOfWork, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
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

        public async Task<string> RefreshAccessToken(string refreshToken)
        {
            try
            {
                await _unitOfWork.OpenConnectionAsync();

                var token = await _unitOfWork.TokenRepository.GetByRefreshToken(refreshToken);
                if (token == null) throw new ArgumentNullException(nameof(token));

                var account = await _unitOfWork.AccountRepository.GetByIdAsync(token.AccountId);
                if (account == null) throw new ArgumentNullException(nameof(account));

                var newAccessToken = CreateJwtToken(account);
                SetJwtCookie(newAccessToken);

                token.AccessTokenExpiresAt = AccessTokenExpieresTime;
                token.AccessTokenHash = newAccessToken.HashToken();

                await _unitOfWork.TokenRepository.UpdateAsync(token);

                return newAccessToken;
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
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]!);
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
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
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