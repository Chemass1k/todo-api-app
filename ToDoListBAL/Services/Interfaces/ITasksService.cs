using ToDoListBAL.Models;
using ToDoListDAL.Data.Entities;

namespace ToDoListBAL.Services.Interfaces
{
    public interface ITasksService
    {
        Task<IEnumerable<Models.TaskItem>> GetTasksByUserId(int userId, TaskQueryParams queryParams);
        Task<ToDoListDAL.Data.Entities.TaskItem> GetTaskItem(int taskId);
        Task<int> AddTask(TaskItemRead task);
        Task<bool> DeleteTask(int id);
        Task<ToDoListDAL.Data.Entities.TaskItem> TaskUpdate(Models.TaskItem task);
    }
}
