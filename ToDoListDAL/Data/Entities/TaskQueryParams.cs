namespace ToDoListDAL.Data.Entities
{
    public class TaskQueryParams
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 5;
        public bool? IsDone { get; set; }
        public string? Search { get; set; }
        public string? Sort { get; set; }
    }
}
