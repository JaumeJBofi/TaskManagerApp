using MongoDB.Driver;
using TaskManagerApi.Models;
using TaskManagerApi.Models.Dtos.TaskManagment;
using TaskManagerApi.Repositories.Interfaces;
using TaskManagerApi.Services.Interfaces;
using TaskManagerApi.Services;
using TaskManagerApi.Utilities;

namespace TaskManagerApi.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly IConfig _config;        
        private readonly IMongoCollection<UserTask> _tasksCollection;
        public TaskRepository(IConfig config, IMongoDatabase mongoDatabase) 
        { 
            _config = config;
             _tasksCollection = mongoDatabase.GetCollection<UserTask>("Tasks"); 

        }
        public Task<UserTask> CreateAsync(CreateTaskDto createTask)
        {
            _tasksCollection.
        }

        public Task<int> DeleteAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task DeleteUserTasksAsync(string userName)
        {
            throw new NotImplementedException();
        }

        public async Task<List<UserTask>> GetAllAsync()
        {
            return await _tasksCollection.Find(_ => true).ToListAsync();
        }

        public async Task<UserTask?> GetByIdAsync(string id)
        {
            return await _tasksCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public Task<List<UserTask>> GetUserTasksAsync(string userName)
        {
            throw new NotImplementedException();
        }

        public Task<UserTask> UpdateAsync(UpdateTaskDto updateTask)
        {
            throw new NotImplementedException();
        }
    }
}
