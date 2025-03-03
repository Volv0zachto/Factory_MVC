using Materials.Data;
using Materials.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Claims;

namespace Materials.Controllers
{
    [Authorize(Roles = "Accountant")]
    public class MaterialRecordsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MaterialRecordsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var materialRecords = await _context.MaterialRecords.ToListAsync();
            return View(materialRecords);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Materials = await _context.Materials.ToListAsync();
            ViewBag.Equipments = await _context.Equipments.ToListAsync();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(MaterialRecord materialRecord)
        {
            ModelState.Remove("Material");
            ModelState.Remove("Equipment");
            ModelState.Remove("UserId");
            ModelState.Remove("User");

            if (ModelState.IsValid)
            {
                var material = await _context.Materials.FindAsync(materialRecord.MaterialId);
                var currentUserName = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
                var dbUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == currentUserName);

                if (material.Quantity >= materialRecord.Quantity)
                {
                    material.Quantity -= materialRecord.Quantity;
                    materialRecord.User = dbUser;
                    materialRecord.RecordDate = materialRecord.RecordDate.ToUniversalTime();

                    await _context.MaterialRecords.AddAsync(materialRecord);

                    var materialLog = new MaterialLog
                    {
                        LogType = LogType.Remove,
                        Material = materialRecord.Material,
                        Quantity = materialRecord.Quantity
                    };

                    await _context.MaterialLogs.AddAsync(materialLog);

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }

                ViewBag.Materials = await _context.Materials.ToListAsync();
                ViewBag.Equipments = await _context.Equipments.ToListAsync();

                return View(materialRecord);
            }

            ViewBag.Materials = await _context.Materials.ToListAsync();
            ViewBag.Equipments = await _context.Equipments.ToListAsync();

            return View(materialRecord);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var materialRecord = await _context.MaterialRecords.FindAsync(id);

            if (materialRecord != null)
            {
                var material = await _context.Materials.FindAsync(materialRecord.MaterialId);
                material.Quantity += materialRecord.Quantity;

                var materialLog = new MaterialLog
                {
                    LogType = LogType.Add,
                    Material = materialRecord.Material,
                    Quantity = materialRecord.Quantity
                };

                await _context.MaterialLogs.AddAsync(materialLog);

                _context.MaterialRecords.Remove(materialRecord);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
