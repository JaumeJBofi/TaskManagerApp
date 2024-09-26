using Microsoft.AspNetCore.Mvc;
using TaskManagerApi.Repositories.Interfaces;
using TaskManagerApi.Utilities;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using TaskManagerApi.Models.Dtos.TaskManagment;
using TaskManagerApi.Models;

namespace TaskManagerApi.Controllers
{
    [Route("api/[controller]/[action]")]    
    public class TaskManagmentController : ControllerBase
    {
        private readonly IConfig _config;
        private readonly IUserRepository _userRepository;
        private readonly ITaskRepository _taskRepository;

        public TaskManagmentController(IConfig config, IUserRepository userRepository, ITaskRepository taskRepository)
        {
            _config = config;
            _userRepository = userRepository;
            _taskRepository = taskRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserTask>>> GetTasks()
        {
            var user = await GetUser();            
            return Ok(await _taskRepository.GetUserTasksAsync(user));
        }
       
        [HttpGet("{id}")]
        public async Task<ActionResult<UserTask>> GetTask(string id)
        {
            var task = await _taskRepository.GetByIdAsync(id);

            if (task == null)
            {
                return NotFound();
            }

            // Authorization check: Only allow access if the task belongs to the logged-in user
            var user = await GetUser();
            if (task.UserId != user.Id)
            {
                return Unauthorized(); 
            }

            return Ok(task);
        }

        [HttpPost]
        [Authorize(Policy = "TaskCreate")] // Example policy for task creation
        public async Task<ActionResult<UserTask>> CreateTask(CreateTaskDto createTaskDto)
        {
            // Get the user's ID from the claims
            var user = await GetUser();

            var newTask = await _taskRepository.CreateAsync(createTaskDto, user);
            return CreatedAtAction(nameof(GetTask), new { id = newTask.Id }, newTask);
        }
       
        [HttpPut("{id}")]
        [Authorize(Policy = "TaskUpdate")] 
        public async Task<IActionResult> UpdateTask(UpdateTaskDto updateTaskDto)
        {       
            var existingTask = await _taskRepository.GetByIdAsync(updateTaskDto.Id);

            if (existingTask == null)
            {
                return NotFound();
            }
            
            var user = await GetUser();
            if (existingTask.UserId != user.Id)
            {
                return Unauthorized();
            }          

            await _taskRepository.UpdateAsync(updateTaskDto);

            return NoContent();
        }
  
        [HttpDelete("{id}")]
        [Authorize(Policy = "TaskDelete")] 
        public async Task<IActionResult> DeleteTask(string id)
        {
            var task = await _taskRepository.GetByIdAsync(id);

            if (task == null)
            {
                return NotFound();
            }

            // Authorization check: Only allow deletion if the task belongs to the logged-in user
            var user = await GetUser();
            if (task.UserId != user.Id)
            {
                return Unauthorized();
            }

            await _taskRepository.DeleteAsync(id);

            return NoContent();
        }

        private async Task<User> GetUser()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new Exception("User id not found in claims");
            var user = await _userRepository.GetByIdAsync(userId);
            return user ?? throw new Exception("User not found");
        }
    }
}
