using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using StoreManager.DTO;
using StoreManager.Facade.Interfaces.Repositories;
using StoreManager.Facade.Interfaces.Services;
using StoreManager.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StoreManager.Services
{
    public class TokenService : ITokenService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TokenService(IUnitOfWork unitOfWork,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor)
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

        public TokenResponse GenerateToken(Account account)
        {
            var token = CreateJwtToken(account.Email);
            var refreshToken = GenerateRefreshToken();
            SetJwtCookie(token);

            return new TokenResponse
            {
                Token = token,
                AccountId = account.Id,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(15)
            };
        }

        private string CreateJwtToken(string email)
        {
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]!);
            var tokenExpiration = DateTime.UtcNow.AddDays(15);

            var claims = new[]
            {
        new Claim("Id", Guid.NewGuid().ToString()),
        new Claim(JwtRegisteredClaimNames.Sub, email),
        new Claim(JwtRegisteredClaimNames.Email, email),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
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

        private string GenerateRefreshToken()
        {
            return Guid.NewGuid().ToString();
        }
    }
}