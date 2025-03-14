using System.ComponentModel.DataAnnotations;

namespace Materials.Models
{
    public class Equipment
    {
        public int EquipmentId { get; set; }

        [Required(ErrorMessage = "Требуется название")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "От 2ух до 100 символов")]
        public string Name { get; set; }

        public virtual ICollection<MaterialRecord>? MaterialRecords { get; set; } = new List<MaterialRecord>();
    }
}
