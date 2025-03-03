using Materials.Models;

namespace Materials.ViewModels
{
    public class AverageMaterialConsumptionReportViewModel
    {
        public Material Material { get; set; }
        public double TotalUsed { get; set; }
        public double AverageConsumptionPerDay { get; set; }
    }
}
