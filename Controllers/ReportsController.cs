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

            var report = await Task.WhenAll(materialsUsage.Select(async item =>
            {
                var material = await _context.Materials.FirstOrDefaultAsync(m => m.MaterialId == item.MaterialId);
                return new MaterialUsageReportViewModel
                {
                    Material = material,
                    TotalUsed = item.TotalUsed,
                };
            }));

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

            var materialConsumption = await _context.MaterialRecords
                .Where(r => r.RecordDate >= startDate && r.RecordDate <= endDate)
                .GroupBy(r => r.MaterialId)
                .Select(g => new
                {
                    MaterialId = g.Key,
                    TotalUsed = g.Sum(r => r.Quantity),
                    DaysInPeriod = (endDate - startDate).Value.Days
                }).ToListAsync();

            var report = await Task.WhenAll(materialConsumption.Select(async item => new AverageMaterialConsumptionReportViewModel
            {
                Material = await _context.Materials.FirstOrDefaultAsync(m => m.MaterialId == item.MaterialId),
                TotalUsed = item.TotalUsed,
                AverageConsumptionPerDay = item.TotalUsed / item.DaysInPeriod
            }).ToList());

            return View(report);
        }
    }
}
