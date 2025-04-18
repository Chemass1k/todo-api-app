using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ToDoListBAL.Models;
using ToDoListBAL.Services.Interfaces;

namespace ToDoList.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequest request)
        {
            try
            {
                var (token, refreshToken) = await _authService.RefreshAsync(request);
                var response = new ApiResponse<object>(true, "Токены успешно обновлены!", new { token, refreshToken });
                return Ok(response);
            }
            catch (SecurityTokenException ex)
            {
                var response = new ApiResponse<string>(false, $"Ошибка при обновлении токенов {ex.Message}", null);
                return Unauthorized(response);
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.RegisterAsync(request);
            var okResponse = new ApiResponse<bool>(true, "Пользователь зарегистрирован!", result);
            var badResponse = new ApiResponse<string>(false, "Пользователь с таким именем существует", null);
            return result ? Ok(okResponse) : BadRequest(badResponse);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var (token, refreshToken) = await _authService.LoginAsync(request);
            var tokens = new { token, refreshToken };
            var response = new ApiResponse<object>(true, "Успешный вход", new { token, refreshToken });
            return Ok(response);
        }
    }
}
