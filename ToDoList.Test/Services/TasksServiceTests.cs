using Castle.Core.Logging;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using ToDoListBAL.Models;
using ToDoListBAL.Services;
using ToDoListDAL.Repositories.Interfaces;

namespace ToDoList.Test.Services
{
    public class TasksServiceTests
    {
        private readonly Mock<ITasksRepository> _repositoryMock;
        private readonly Mock<ILogger<TasksService>> _loggerMock;
        private readonly TasksService _service;

        public TasksServiceTests()
        {
            _repositoryMock = new Mock<ITasksRepository>();
            _loggerMock = new Mock<ILogger<TasksService>>();
            _service = new TasksService(_repositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetTasksByUserId_ShouldReturnMappedTasks_WhenRepositoryReturnsData()
        {
            int userId = 1;
            var taskParams1 = new ToDoListDAL.Data.Entities.TaskQueryParams
            {
                IsDone = true,
                Search = null,
                Sort = "desc",
            };

            var item1 = new ToDoListDAL.Data.Entities.TaskItem { Id = 1, Title = "Test1", IsDone = true };
            var item2 = new ToDoListDAL.Data.Entities.TaskItem { Id = 2, Title = "Test2", IsDone = false };
            var repoTasks = new List<ToDoListDAL.Data.Entities.TaskItem> { item1, item2 };

            _repositoryMock
                .Setup(r => r.GetTasksByUserId(userId, taskParams1))
                .ReturnsAsync(new List<ToDoListDAL.Data.Entities.TaskItem> { item1 });


            var result = await _service.GetTasksByUserId(userId, taskParams1);

            result.Should().HaveCount(1);
            result.Should().ContainSingle(t => t.Id == 1 && t.IsDone == true);

            //result.Should().HaveCount(2);
            //result.Should().ContainSingle(t => t.Title == "Test1" && t.IsDone == true);
            //result.Should().ContainSingle(t => t.Title == "Test2" && t.IsDone == false);
        }

        [Fact]
        public async Task AddTask_ShouldReturnTaskId_WhenTaskIsValid()
        {
            var taskToAdd = new TaskItemRead
            {
                Title = "New Test",
                IsDone = true,
                UserId = 1
            };

            var savedTask = new ToDoListDAL.Data.Entities.TaskItem
            {
                Id = 10,
                Title = taskToAdd.Title,
                IsDone = taskToAdd.IsDone,
                UserId = taskToAdd.UserId
            };

            _repositoryMock
                .Setup(r => r.AddTask(It.IsAny<ToDoListDAL.Data.Entities.TaskItem>()))
                .Callback<ToDoListDAL.Data.Entities.TaskItem>(task => task.Id = savedTask.Id)
                .ReturnsAsync(savedTask.Id);

            var result = await _service.AddTask(taskToAdd);

            result.Should().Be(10);
        }

        [Fact]
        public async Task DeleteTask_ShouldReturnTrue_WhenRepositoryReturnsResult()
        {
            int id = 10;
            _repositoryMock
                .Setup(r => r.DeleteTask(id))
                .ReturnsAsync(true);

            var result = await _service.DeleteTask(id);
            result.Should().BeTrue();
        }
        [Fact]
        public async Task GetTaskItem_ShouldReturnTask_WhenTaskExist()
        {
            int id = 10;

            var repoResult = new ToDoListDAL.Data.Entities.TaskItem
            {
                Id = id,
                Title = "Test",
                IsDone = true,
                UserId = 1
            };

            _repositoryMock
                .Setup(r => r.GetTaskById(id))
                .ReturnsAsync(repoResult);

            var result = await _service.GetTaskItem(id);
            result.Should().BeEquivalentTo(repoResult);
        }

        [Fact]
        public async Task TaskUpdate_ShouldReturnUpdatedTask_WhenTaskExist()
        {
            var repoTaskBAL = new ToDoListBAL.Models.TaskItem
            {
                Id = 10,
                Title = "Test",
                IsDone = true,
                UserId = 1
            };

            var repoTaskDAL = new ToDoListDAL.Data.Entities.TaskItem
            {
                Id = 10,
                Title = "Test",
                IsDone = true,
                UserId = 1
            };

            var updatedTaskDAL = new ToDoListDAL.Data.Entities.TaskItem
            {
                Id = 10,
                Title = "Updated Test",
                IsDone = true,
                UserId = 1
            };


            var updatedTaskBAL = new ToDoListBAL.Models.TaskItem
            {
                Id = 10,
                Title = "Updated Test",
                IsDone = true,
                UserId = 1
            };

            _repositoryMock
                .Setup(r => r.UpdateTask(It.Is<ToDoListDAL.Data.Entities.TaskItem>(t =>
                t.Id == repoTaskDAL.Id &&
                t.Title == repoTaskDAL.Title &&
                t.IsDone == repoTaskDAL.IsDone)))
                .ReturnsAsync(updatedTaskDAL);

            var result = await _service.TaskUpdate(repoTaskBAL);
            result.Should().BeEquivalentTo(updatedTaskBAL);


        }
    }
}
