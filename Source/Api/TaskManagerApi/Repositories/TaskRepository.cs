using MongoDB.Driver;
using TaskManagerApi.Models;
using TaskManagerApi.Models.Dtos.TaskManagment;
using TaskManagerApi.Repositories.Interfaces;
using TaskManagerApi.Utilities;
using MongoDB.Bson;

namespace TaskManagerApi.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly IConfig _config;        
        private readonly IMongoCollection<UserTask> _tasksCollection;
        private readonly IUserRepository _userRepository;
        public TaskRepository(IConfig config, IMongoDatabase mongoDatabase,IUserRepository userRepository) 
        { 
            _config = config;
            _userRepository = userRepository;
             _tasksCollection = mongoDatabase.GetCollection<UserTask>("Tasks"); 

        }
        public async Task<UserTask> CreateAsync(CreateTaskDto createTask, User user)
        {
            var newTask = new UserTask
            {
                Id = ObjectId.GenerateNewId().ToString(),
                UserId = user.Id,
                Title = createTask.Title,
                Description = createTask.Description,
                DueDate = createTask.DueDate,
                Status = (TASK_STATUS)createTask.Status
            };

            await _tasksCollection.InsertOneAsync(newTask);

            return newTask;
        }

        public async Task<int> DeleteAsync(string id)
        {
            var deleteResult = await _tasksCollection.DeleteOneAsync(x => x.Id == id);
            return (int)deleteResult.DeletedCount;
        }

        public async Task<int> DeleteUserTasksAsync(User user)
        {            
            var deleteResult = await _tasksCollection.DeleteManyAsync(x => x.UserId == user.Id);            
            return (int)deleteResult.DeletedCount;
        }

        public async Task<List<UserTask>> GetAllAsync()
        {
            return await _tasksCollection.Find(_ => true).ToListAsync();
        }

        public async Task<UserTask?> GetByIdAsync(string id)
        {
            return await _tasksCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<UserTask>> GetUserTasksAsync(User user)
        {
            return await _tasksCollection.Find(x => x.UserId == user.Id).ToListAsync();
        }

        public async Task<UserTask> UpdateAsync(UpdateTaskDto updateTask)
        {
            var filter = Builders<UserTask>.Filter.Eq(x => x.Id, updateTask.Id);
            var update = Builders<UserTask>.Update.Combine(); 

            if (updateTask.Title != null)
                update = update.Set(x => x.Title, updateTask.Title);

            if (updateTask.Description != null)
                update = update.Set(x => x.Description, updateTask.Description);

            if (updateTask.DueDate != null)
                update = update.Set(x => x.DueDate, updateTask.DueDate);

            if (updateTask.Status != null)
                update = update.Set(x => x.Status, updateTask.Status);

            return await _tasksCollection.FindOneAndUpdateAsync(filter, update, new FindOneAndUpdateOptions<UserTask> { ReturnDocument = ReturnDocument.After });
        }
    }
}
