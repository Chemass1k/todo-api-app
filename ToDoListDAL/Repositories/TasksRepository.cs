using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using ToDoListDAL.Data;
using ToDoListDAL.Data.Entities;
using ToDoListDAL.Repositories.Interfaces;

namespace ToDoListDAL.Repositories
{
    public class TasksRepository : ITasksRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<TasksRepository> _log;

        public TasksRepository(AppDbContext dbContext, ILogger<TasksRepository> log)
        {
            _dbContext = dbContext;
            _log = log;
        }

        public async Task<int> AddTask(TaskItem task)
        {
            try
            {
                _log.LogInformation($"Adding new task for user {task.UserId} in DB");
                await _dbContext.Tasks.AddAsync(task);
                await _dbContext.SaveChangesAsync();
                var _task = await _dbContext.Tasks.OrderBy(i => i.Id).LastAsync();
                _log.LogInformation($"Task for user {task.UserId} added successfuly");
                return _task.Id;
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error in process adding task to DB");
                throw;
            }
        }

        public async Task<IEnumerable<TaskItem>> GetTasksByUserId(int userId, TaskQueryParams? queryParams = null)
        {
            try
            {
                _log.LogInformation($"Getting tasks list for user: {userId}");
                var query = _dbContext.Tasks.AsQueryable();

                query = query.Where(t => t.UserId == userId);

                if (queryParams.IsDone.HasValue)
                    query = query.Where(t => t.IsDone == queryParams.IsDone.Value);

                if (!string.IsNullOrWhiteSpace(queryParams.Search))
                    query = query.Where(t => t.Title.Contains(queryParams.Search));

                if (queryParams.Sort?.ToLower() == "desc")
                    query = query.OrderByDescending(t => t.Id);
                else
                    query = query.OrderBy(t => t.Id);

                int skip = (queryParams.Page - 1) * queryParams.PageSize;

                var pagedTasks = await query
                    .Skip(skip)
                    .Take(queryParams.PageSize)
                    .ToListAsync();

                return pagedTasks;
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "GetTasksByUserId finished with error");
                throw;
            }
        }

        public async Task<TaskItem> GetTaskById(int id)
        {
            try
            {
                _log.LogInformation($"Trying to find task with ID {id} in DB!");
                var task = await _dbContext.Tasks.FirstOrDefaultAsync(x => x.Id == id);
                if (task != null)
                {
                    _log.LogInformation($"Task with ID {id} is found in DB!");
                    return task;
                }
                _log.LogWarning($"Task with ID {id} isn't found in DB!");
                return null;
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error during find task in DB!");
                throw;
            }
        }

        public async Task<bool> DeleteTask(int id)
        {
            try
            {
                _log.LogInformation($"Task with ID {id} deleted");
                var task = await _dbContext.Tasks.FirstOrDefaultAsync(i => i.Id == id);
                if (task != null)
                {
                    _dbContext.Tasks.Remove(task);
                    await _dbContext.SaveChangesAsync();
                    _log.LogInformation($"Task deleted succsessfuly from DB!");
                    return true;
                }
                else
                {
                    _log.LogWarning($"Task with {id} wasn't found in DB for delete!");
                    return false;
                }

            }
            catch (Exception ex)
            {
                {
                    _log.LogError(ex, "Error deleting task from DB!");
                    return false;
                }
            }
        }

        public async Task<TaskItem> UpdateTask(TaskItem task)
        {
            try
            {
                _log.LogInformation($"Updating task with id {task.Id} proccess started for user {task.UserId}");
                _dbContext.Tasks.Update(task);
                await _dbContext.SaveChangesAsync();
                _log.LogInformation($"Task with ID {task.Id} for user {task.UserId} updated!");
                return task;
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error updating task in DB!");
                throw;
            }
        }
    }
}
