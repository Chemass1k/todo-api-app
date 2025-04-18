using System.ComponentModel.DataAnnotations;

namespace ToDoListBAL.Models
{
    public class TaskItemCreate
    {
        [Required(ErrorMessage="Поле 'Title' обязательно!")]
        [MaxLength(100, ErrorMessage ="Максимум 100 символов!")]
        public string Title { get; set; }
        public bool IsDone { get; set; }
       // public int UserId { get; set; }
    }
}
