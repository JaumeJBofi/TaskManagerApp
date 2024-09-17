namespace TaskManagerApi.Models.Dtos.TaskManagment
{
    public class UpdateTaskDto
    {
        public required string Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public TASK_STATUS? Status { get; set; }
        public DateTime? DueDate { get; set; }
    }
}
