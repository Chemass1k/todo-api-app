namespace ToDoListBAL.Models
{
    public class TaskItemRead
    {
        public string Title { get; set; }
        public bool IsDone { get; set; }
        public int UserId { get; set; }
    }
}
