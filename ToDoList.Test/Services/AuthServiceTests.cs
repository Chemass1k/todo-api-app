using Azure.Core;
using Castle.Core.Logging;
using FluentAssertions;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.Extensions.Logging;
using Moq;
using ToDoListBAL.Models;
using ToDoListBAL.Services;
using ToDoListDAL.Data.Entities;
using ToDoListDAL.Repositories.Interfaces;

namespace ToDoList.Test.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IAuthRepository> _repositoryMock;
        private readonly Mock<ILogger<AuthService>> _loggerMock;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _repositoryMock = new Mock<IAuthRepository>();
            _loggerMock = new Mock<ILogger<AuthService>>();
            _authService = new AuthService(_repositoryMock.Object);
        }

        [Fact]
        public async Task Register_ShouldReturnTrue_WhenUsernameIsNotTakenOrPasswordsTheSame()
        {
            var requestBAL = new ToDoListBAL.Models.RegisterRequest
            {
                Username = "1234",
                Password = "!123Ar",
                PasswordConfirmation = "!123Ar"
            };

            var requestDAL = new User
            {
                Username = requestBAL.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(requestBAL.Password),
                Role = "User",
                RefreshToken = null,
                RefreshTokenExpiryTime = DateTime.UtcNow,
                Tasks = new List<ToDoListDAL.Data.Entities.TaskItem>(),
                Id = 1
            };

            _repositoryMock
                .Setup(r => r.RegisterAsync(It.IsAny<User>()))
                .ReturnsAsync(true);

            //u =>
            //    u.Username == requestBAL.Username &&
            //    u.Role == "User" &&
            //    u.Tasks != null
            //    ))

            var result = await _authService.RegisterAsync(requestBAL);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task Login_ShouldReturnTokens_WhenUsernameAndPasswordCorrect()
        {
            var user = new ToDoListBAL.Models.LoginRequest
            {
                Username = "chema",
                Password = "!123Ar"
            };

            _repositoryMock
                .Setup(r => r.LoginAsync(user.Username, user.Password))
                .ReturnsAsync(("access-token", "refresh-token"));

            var result = await _authService.LoginAsync(user);

            result.Token.Should().NotBeNullOrWhiteSpace();
            result.RefreshToken.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task RefreshToken_ShouldReturnNewTokens_WhenValid()
        {
            var refreshToken = "valid-refresh-token";
            var request = new ToDoListBAL.Models.RefreshRequest
            {
                RefreshToken = refreshToken
            };

            var expectedAccess = "new-access-token";
            var expectedRefresh = "new-refresh-token";

            _repositoryMock
                .Setup(r => r.RefreshTokensAsync(refreshToken))
                .ReturnsAsync((expectedAccess, expectedRefresh));

            var result = await _authService.RefreshAsync(request);
            result.Token.Should().Be(expectedAccess);
            result.RefreshToken.Should().Be(expectedRefresh);
        }
    }
}
