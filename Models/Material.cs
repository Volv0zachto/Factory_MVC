using System.ComponentModel.DataAnnotations;

namespace Materials.Models
{
    public class Material
    {
        public int MaterialId { get; set; }

        [Required(ErrorMessage = "Требуется ввести название")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "От 2ух до 100 символов")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Требуется измерение величин")]
        [StringLength(10, ErrorMessage = "Максимально 10 символов")]
        public string Unit { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Количество должно быть больше 0")]
        public double Quantity { get; set; }

        public virtual ICollection<MaterialRecord>? MaterialRecords { get; set; } = new List<MaterialRecord>();
    }
}