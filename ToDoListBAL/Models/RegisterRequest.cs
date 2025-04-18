using System.ComponentModel.DataAnnotations;

namespace ToDoListBAL.Models
{
    public class RegisterRequest : IValidatableObject
    {
        [Required(ErrorMessage = "Username is required!")]
        [MaxLength(50, ErrorMessage = "Длина не может быть больше 50 символов")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Password is required!")]
        [MinLength(5, ErrorMessage = "Длина не может быть меньше 5 символов")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Password confirmation is required!")]
        [MinLength(5, ErrorMessage = "Длина не может быть меньше 5 символов")]
        public string PasswordConfirmation { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Password != PasswordConfirmation)
            {
                yield return new ValidationResult(
                    "Пароли не совпадают!",
                    new[] { nameof(PasswordConfirmation) });
            }
        }
    }
}
