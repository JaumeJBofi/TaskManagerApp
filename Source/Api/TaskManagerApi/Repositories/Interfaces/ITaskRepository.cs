using TaskManagerApi.Models.Dtos.Authentication;
using TaskManagerApi.Models;
using TaskManagerApi.Models.Dtos.TaskManagment;

namespace TaskManagerApi.Repositories.Interfaces
{
    public interface ITaskRepository
    {
        Task<List<UserTask>> GetAllAsync();
        Task<UserTask?> GetByIdAsync(string id);
        Task<List<UserTask>> GetUserTasksAsync(User user);
        Task<UserTask> CreateAsync(CreateTaskDto createTask, User user);
        Task<int> DeleteAsync(string id);
        Task<int> DeleteUserTasksAsync(User user);
        Task<UserTask> UpdateAsync(UpdateTaskDto updateTask);
    }
}
