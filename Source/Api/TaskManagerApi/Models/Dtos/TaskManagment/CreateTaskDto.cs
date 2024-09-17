namespace TaskManagerApi.Models.Dtos.TaskManagment
{
    public class CreateTaskDto
    {
        public required string Title {  get; set; }
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }
        public required int Status { get; set; }
        public required string UserName { get; set; }
    }
}
