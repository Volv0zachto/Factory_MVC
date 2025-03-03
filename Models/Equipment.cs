using System.ComponentModel.DataAnnotations;

namespace Materials.Models
{
    public class Equipment
    {
        public int EquipmentId { get; set; }
        public string Name { get; set; }
        public virtual ICollection<MaterialRecord>? MaterialRecords { get; set; } = new List<MaterialRecord>();
    }
}
