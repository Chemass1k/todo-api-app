using ToDoListDAL.Data.Entities;

namespace ToDoListDAL.Repositories.Interfaces
{
    public interface ITasksRepository
    {
        Task<IEnumerable<TaskItem>> GetTasksByUserId(int userId, TaskQueryParams? queryParams = null);
        Task<TaskItem> GetTaskById(int id);
        Task<int> AddTask(TaskItem task);
        Task<bool> DeleteTask(int id);
        Task<TaskItem> UpdateTask(TaskItem task);
    }
}
