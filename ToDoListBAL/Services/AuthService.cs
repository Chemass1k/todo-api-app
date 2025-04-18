using Microsoft.Extensions.Logging;
using ToDoListBAL.Services.Interfaces;
using ToDoListDAL.Data.Entities;
using ToDoListDAL.Repositories.Interfaces;

namespace ToDoListBAL.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly ILogger<AuthService> _log;

        public AuthService(IAuthRepository authRepository, ILogger<AuthService> log) 
        {
            _authRepository = authRepository;
            _log = log;
        }
        public async Task<bool> RegisterAsync(Models.RegisterRequest request)
        {
            try
            {
                _log.LogInformation($"Sending request to register user {request.Username}");
                var user = new User
                {
                    Username = request.Username,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                    Role = "User"
                };

                var result = await _authRepository.RegisterAsync(user);
                _log.LogInformation($"Got successful response. User {user.Username} registered!");
                return result;
            }
            catch (Exception ex)
            {
                _log.LogError($"Resived error response after try register user {request.Username}. Error: {ex.Message}");
                return false;
            }
        }

        public async Task<(string Token, string RefreshToken)> LoginAsync(Models.LoginRequest request)
        {
            try
            {
                _log.LogInformation($"Sending request to sign in user {request.Username}");
                var result = await _authRepository.LoginAsync(request.Username, request.Password);
                _log.LogInformation($"User {request.Username} signed in");
                return result;
            }
            catch (Exception ex)
            {
                _log.LogError($"Login failed: {ex.Message}");
                return (null, null);
            }
        }

        public Task<(string Token, string RefreshToken)> RefreshAsync(Models.RefreshRequest request)
        {
            return _authRepository.RefreshTokensAsync(request.RefreshToken);
        }
    }
}
