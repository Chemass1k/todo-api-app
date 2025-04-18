using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoListBAL.Models;
using ToDoListBAL.Services.Interfaces;
using ToDoListDAL.Data.Entities;

namespace ToDoList.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class TasksController : Controller
    {
        private readonly ITasksService _service;
        private readonly ILogger<TasksController> _logger;

        public TasksController(ITasksService service, ILogger<TasksController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll([FromQuery] TaskQueryParams queryParams)
        {
            _logger.LogInformation("GetAll() working");
            var userId = int.Parse(User.FindFirst("userId")!.Value);
            var tasks = await _service.GetTasksByUserId(userId, queryParams);
            if (tasks != null)
            {
                var response = new ApiResponse<IEnumerable<ToDoListBAL.Models.TaskItem>>(true, "Задачи успешно получены", tasks);
                return Ok(response);
            }
            else
            {
                var response = new ApiResponse<string>(false, "Нет задач!", null);
                return NotFound(response);
            }
                
        }

        [HttpGet("get-task/{id}")]
        public async Task<IActionResult> GetTask(int id)
        {
            var result = await _service.GetTaskItem(id);
            var okResponse = new ApiResponse<ToDoListDAL.Data.Entities.TaskItem>(true, "Задача получена!", result);
            var badResponse = new ApiResponse<string>(false, "Задача не найдена!", null);

            if (result != null)
                return Ok(okResponse);
            else
                return NotFound(badResponse);
        }

        [HttpPost("add-task")]
        public async Task<IActionResult> AddTask([FromBody]ToDoListBAL.Models.TaskItemCreate item)
        {
            var userId = int.Parse(User.FindFirst("userId")!.Value);
            _logger.LogInformation($"Task Title: {item.Title}");
            var task = new ToDoListBAL.Models.TaskItemRead { Title = item.Title, IsDone = item.IsDone, UserId = userId };
            var id = await _service.AddTask(task);
            var badResponse = new ApiResponse<string>(false, "Задача не создана!", null);
            var okResponse = new ApiResponse<ToDoListBAL.Models.TaskItemCreate>(true, "Задача создана!", item);
            if (id != 0)
                return CreatedAtAction(nameof(GetTask), new { id }, okResponse);
            return BadRequest(badResponse);
        }

        [HttpDelete("delete-task/{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var result = await _service.DeleteTask(id);
            var okResponse = new ApiResponse<bool>(true, "Задача удалена!", result);
            var badResponse = new ApiResponse<string>(false, "Произошла ошибка при попытке удаления!", null);
            if (result)
                return Ok(okResponse);
            return BadRequest(badResponse);
        }

        [HttpPut("update-task")]
        public async Task<IActionResult> UpdateTask([FromBody]ToDoListBAL.Models.TaskItem item)
        {
            var userId = int.Parse(User.FindFirst("userId")!.Value);
            _logger.LogInformation($"userId: {userId}");
            var task = new ToDoListBAL.Models.TaskItem
            {
                Id = item.Id,
                Title = item.Title,
                IsDone = item.IsDone,
                UserId = userId
            };
            var result = await _service.TaskUpdate(task);
            if (result != null)
            {
                var response = new ApiResponse<ToDoListDAL.Data.Entities.TaskItem>(true, "Задача обновлена!", result);
                return Ok(response);
            }

            return BadRequest(new ApiResponse<string>(false, "Ошибка при обновлении задачи!", null));
        }
    }
}
