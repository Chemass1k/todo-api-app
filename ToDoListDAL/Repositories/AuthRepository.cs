using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ToDoListDAL.Data;
using ToDoListDAL.Data.Entities;
using ToDoListDAL.Repositories.Interfaces;

namespace ToDoListDAL.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly AppDbContext _appDbContext;
        private readonly IConfiguration _config;
        private readonly ILogger<AuthRepository> _log;

        public AuthRepository(AppDbContext appDbContext, IConfiguration config, ILogger<AuthRepository> log)
        {
            _config = config;
            _appDbContext = appDbContext;
            _log = log;
        }

        public async Task<(string AccessToken, string RefreshToken)> LoginAsync(string username, string password)
        {
            try
            {
                var user = await _appDbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
                _log.LogInformation($"Signing user {user.Id} in...");
                if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                {
                    _log.LogWarning("Incorect password!");
                    return (null, null);
                }

                var claims = new[]
                {
                new Claim("userId", user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role),
            };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: _config["Jwt:Issuer"],
                    audience: _config["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddHours(1),
                    signingCredentials: creds
                    );

                var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
                var refreshToken = GenerateRefreshToken();
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

                await _appDbContext.SaveChangesAsync();
                _log.LogInformation($"User {user.Id} signed in successfully!");
                return (accessToken, refreshToken);
            }
            catch (Exception ex)
            {
                _log.LogError($"Can not to login. Error: {ex}");
                return (null, null);
            }
        }

        public async Task<bool> RegisterAsync(User user)
        {
            try
            {
                _log.LogInformation($"Trying to sign up user {user.Username} ");
                if (await _appDbContext.Users.AnyAsync(u => u.Username == user.Username))
                {
                    _log.LogWarning($"User with username {user.Username} alreadt exist");
                    return false;
                }


                _appDbContext.Users.Add(user);
                await _appDbContext.SaveChangesAsync();
                _log.LogInformation($"User {user.Username} has registered!");
                return true;
            }
            catch (Exception ex)
            {
                _log.LogError($"Error during sign up proccess. Error: {ex}");
                return false;
            }
        }

        public string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        public async Task<(string AccessToken, string RefreshToken)> RefreshTokensAsync(string refreshToken)
        {
            try
            {
                var user = await _appDbContext.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

                _log.LogInformation($"Generating new tokens");

                if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                {
                    _log.LogWarning($"Invalid or expired refresh token");
                    throw new SecurityTokenException("Invalid or expired refresh token");
                }

                var claims = new[]
                {
                new Claim("userId", user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role)
            };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: _config["Jwt:Issuer"],
                    audience: _config["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(15),
                    signingCredentials: creds
                );

                var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
                var newRefreshToken = GenerateRefreshToken();

                user.RefreshToken = newRefreshToken;
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

                await _appDbContext.SaveChangesAsync();

                return (accessToken, newRefreshToken);
            }
            catch (Exception ex)
            {
                _log.LogError($"Error refreshing tokens! Error: {ex}");
                return (null, null);
            }
        }

    }
}
