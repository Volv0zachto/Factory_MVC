using System.ComponentModel.DataAnnotations;

namespace Materials.Models
{
    public class Material
    {
        public int MaterialId { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        public double Quantity { get; set; }
        public virtual ICollection<MaterialRecord>? MaterialRecords { get; set; } = new List<MaterialRecord>();
    }
}
