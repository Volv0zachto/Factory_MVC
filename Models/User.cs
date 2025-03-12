using System.ComponentModel.DataAnnotations;

namespace Materials.Models
{
    public class User
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "Введите имя пользователя")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Имя пользователя должно быть от 3 до 50 символа")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Введите пароль")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Пароль должен быть не менее 1 символа")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Выберите роль")]
        public int RoleId { get; set; }

        public virtual Role Role { get; set; }
        public virtual ICollection<MaterialRecord>? MaterialRecords { get; set; } = new List<MaterialRecord>();
    }
}