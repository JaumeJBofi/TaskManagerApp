using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace TaskManagerApi.Models.Dtos
{
    public class UserTaskDto
    {
        public UserTaskDto(UserTask task)
        {
            Id = task.Id;
            Title = task.Title;
            Description = task.Description;
            DueDate = task.DueDate;
            Status = task.Status;
        }
        public string Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }        
        public TASK_STATUS Status { get; set; }
    }
}
