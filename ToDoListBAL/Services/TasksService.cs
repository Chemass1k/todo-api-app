using Microsoft.Extensions.Logging;
using Serilog;
using System.Threading.Tasks;
using ToDoListBAL.Models;
using ToDoListBAL.Services.Interfaces;
using ToDoListDAL.Data.Entities;
using ToDoListDAL.Repositories.Interfaces;

namespace ToDoListBAL.Services
{
    public class TasksService : ITasksService
    {
        private readonly ITasksRepository _repository;
        private readonly ILogger<TasksService> _logger;

        public TasksService(ITasksRepository repository, ILogger<TasksService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<int> AddTask(TaskItemRead task)
        {
            _logger.LogInformation($"Creating new task for user {task.UserId}");
            try
            {
                var _task = new ToDoListDAL.Data.Entities.TaskItem
                {
                    Title = task.Title,
                    IsDone = task.IsDone,
                    UserId = task.UserId
                };
                await _repository.AddTask(_task);
                _logger.LogInformation($"Task created with ID {_task.Id}");
                return _task.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to create task for user {task.UserId}");
                throw;
            }
        }

        public async Task<bool> DeleteTask(int id)
        {
            _logger.LogInformation($"Deleting a task with ID {id}");
            try
            {
                var result = await _repository.DeleteTask(id);
                _logger.LogInformation($"Task with ID {id} deleted!");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error during delete task with ID {id}");
                throw;
            }
        }

        public async Task<ToDoListDAL.Data.Entities.TaskItem> GetTaskItem(int taskId)
        {
            _logger.LogInformation($"Getting a task with ID {taskId}");
            try
            {
                var result = await _repository.GetTaskById(taskId);

                if (result != null)
                    _logger.LogInformation($"Task with ID {taskId} has found successfuly!");
                else
                    _logger.LogWarning($"Task with ID {taskId} isn't exist!");

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting task with ID {taskId}");
                throw;
            }
        }

        public async Task<IEnumerable<Models.TaskItem>> GetTasksByUserId(int userId, TaskQueryParams? queryParams)
        {
            try
            {
                _logger.LogInformation($"Request to get tasks for user {userId}");
                var entities = await _repository.GetTasksByUserId(userId, queryParams);

                return entities.Select(t => new Models.TaskItem
                {
                    Id = t.Id,
                    Title = t.Title,
                    IsDone = t.IsDone,
                });
                //var _tasks = await _repository.GetTasksByUserId(userId);
                //var _taskList = new List<Models.TaskItem>();
                //foreach (var task in _tasks)
                //{
                //    _taskList.Add(new Models.TaskItem
                //    {
                //        Id = task.Id,
                //        Title = task.Title,
                //        IsDone = task.IsDone
                //    });
                //}
                //_logger.LogInformation($"Tasks for user {userId} have found successfuly");
                //return _taskList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error trying to get tasks for user with ID: {userId}");
                throw;
            }
        }

        public async Task<ToDoListDAL.Data.Entities.TaskItem> TaskUpdate(Models.TaskItem task)
        {
            try
            {
                _logger.LogInformation($"Updating task with id {task.Id} for user {task.UserId}");
                var _task = new ToDoListDAL.Data.Entities.TaskItem
                {
                    Id = task.Id,
                    Title = task.Title,
                    IsDone = task.IsDone,
                    UserId  = task.UserId
                };
                var result = await _repository.UpdateTask(_task);
                _logger.LogInformation($"Task with ID {_task.Id} for user {_task.UserId} is updated!");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error during update the task");
                throw;
            }
        }
    }
}
