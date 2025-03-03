namespace Materials.Models
{
    public class MaterialLog
    {
        public int MaterialLogId { get; set; }
        public LogType LogType { get; set; }
        public virtual Material Material { get; set; }
        public double Quantity { get; set; }
    }

    public enum LogType
    {
        Add,
        Remove
    }
}
