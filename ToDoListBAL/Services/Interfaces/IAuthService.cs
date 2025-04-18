using ToDoListBAL.Models;

namespace ToDoListBAL.Services.Interfaces
{
    public interface IAuthService
    {
        Task<bool> RegisterAsync(Models.RegisterRequest request);
        Task<(string Token, string RefreshToken)> LoginAsync(LoginRequest request);
        Task<(string Token, string RefreshToken)> RefreshAsync(RefreshRequest request);
    }
}
