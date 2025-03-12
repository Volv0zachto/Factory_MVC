using System.ComponentModel.DataAnnotations;

namespace Materials.Models
{
    public class MaterialRecord
    {
        [Key]
        public int RecordId { get; set; }

        [Required(ErrorMessage = "Требуется материал")]
        public int MaterialId { get; set; }

        [Required(ErrorMessage = "Требуется оборудование")]
        public int EquipmentId { get; set; }

        [Required(ErrorMessage = "Требуется пользователь")]
        public int UserId { get; set; }
        [Required(ErrorMessage = "Требуется ввести количество")]
        [Range(0.1, double.MaxValue, ErrorMessage = "Количество должно быть больше 0")]
        public double Quantity { get; set; }

        [Required(ErrorMessage = "Требуется дата")]
        public DateTime RecordDate { get; set; }

        public virtual Material Material { get; set; }
        public virtual Equipment Equipment { get; set; }
        public virtual User User { get; set; }
    }
}