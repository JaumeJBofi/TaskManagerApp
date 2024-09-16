using TaskManagerApi.Models.Dtos.Authentication;
using TaskManagerApi.Models;
using TaskManagerApi.Models.Dtos.TaskManagment;

namespace TaskManagerApi.Repositories.Interfaces
{
    public interface ITaskRepository
    {
        Task<List<UserTask>> GetAllAsync();
        Task<UserTask?> GetByIdAsync(string id);
        Task<List<UserTask>> GetUserTasksAsync(string userName);
        Task<UserTask> CreateAsync(CreateTaskDto createTask);
        Task<int> DeleteAsync(string id);
        Task DeleteUserTasksAsync(string userName);
        Task<UserTask> UpdateAsync(UpdateTaskDto updateTask);
    }
}
