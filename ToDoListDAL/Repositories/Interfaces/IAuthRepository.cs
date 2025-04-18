using ToDoListDAL.Data.Entities;

namespace ToDoListDAL.Repositories.Interfaces
{
    public interface IAuthRepository
    {
        Task<bool> RegisterAsync(User user);
        Task<(string AccessToken, string RefreshToken)> LoginAsync(string username, string password);
        string GenerateRefreshToken();
        Task<(string AccessToken, string RefreshToken)> RefreshTokensAsync(string refreshToken);

    }
}
