using Materials.Data;
using Materials.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Materials.Controllers
{
    [Authorize(Roles = "Accountant, Boss")]
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> MaterialUsageReport(DateTime? startDate, DateTime? endDate)
        {
            if (startDate == null || endDate == null)
            {
                startDate = DateTime.UtcNow.AddMonths(-1);
                endDate = DateTime.UtcNow;
            }
            else
            {
                startDate = startDate.Value.ToUniversalTime();
                endDate = endDate.Value.ToUniversalTime();
            }

            var materialsUsage = await _context.MaterialRecords
                .Where(r => r.RecordDate >= startDate && r.RecordDate <= endDate)
                .GroupBy(r => r.MaterialId)
                .Select(g => new
                {
                    MaterialId = g.Key,
                    TotalUsed = g.Sum(r => r.Quantity)
                }).ToListAsync();

            var materialIds = materialsUsage.Select(m => m.MaterialId).ToList();
            var materials = await _context.Materials
                .Where(m => materialIds.Contains(m.MaterialId))
                .ToDictionaryAsync(m => m.MaterialId);

            var report = materialsUsage.Select(item => new MaterialUsageReportViewModel
            {
                Material = materials.TryGetValue(item.MaterialId, out var material) ? material : null,
                TotalUsed = item.TotalUsed,
            }).ToList();

            return View(report);
        }


        public async Task<ActionResult> AverageMaterialConsumptionReport(DateTime? startDate, DateTime? endDate)
        {
            if (startDate == null || endDate == null)
            {
                startDate = DateTime.UtcNow.AddMonths(-1);
                endDate = DateTime.UtcNow;
            }
            else
            {
                startDate = startDate.Value.ToUniversalTime();
                endDate = endDate.Value.ToUniversalTime();
            }

            int daysInPeriod = (endDate - startDate).Value.Days;
    
            if (daysInPeriod == 0) daysInPeriod = 1; 

            var materialConsumption = await _context.MaterialRecords
                .Where(r => r.RecordDate >= startDate && r.RecordDate <= endDate)
                .GroupBy(r => r.MaterialId)
                .Select(g => new
                {
                    MaterialId = g.Key,
                    TotalUsed = g.Sum(r => r.Quantity)
                }).ToListAsync();

            var materialIds = materialConsumption.Select(m => m.MaterialId).ToList();
            var materials = await _context.Materials
                .Where(m => materialIds.Contains(m.MaterialId))
                .ToDictionaryAsync(m => m.MaterialId);

            var report = materialConsumption.Select(item => new AverageMaterialConsumptionReportViewModel
            {
                Material = materials.TryGetValue(item.MaterialId, out var material) ? material : null,
                TotalUsed = item.TotalUsed,
                AverageConsumptionPerDay = (double)item.TotalUsed / daysInPeriod
            }).ToList();

            return View(report);
        }

    }
}
