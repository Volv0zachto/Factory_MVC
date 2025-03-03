using System.ComponentModel.DataAnnotations;

namespace Materials.Models
{
    public class MaterialRecord
    {
        [Key]
        public int RecordId { get; set; }
        public int MaterialId { get; set; }
        public virtual Material Material { get; set; }
        public int EquipmentId { get; set; }
        public virtual Equipment Equipment { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public double Quantity { get; set; }
        public DateTime RecordDate { get; set; }
    }
}
