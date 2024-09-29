namespace TaskManagerApi.Models.Dtos.TaskManagment
{
    public class CreateTaskDto
    {
        public required string Title {  get; set; }
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }
        public required TASK_STATUS Status { get; set; }        
    }
}
