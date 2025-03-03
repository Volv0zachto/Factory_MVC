using Materials.Data;
using Materials.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Materials.Controllers
{
    [Authorize(Roles = "Accountant")]
    public class MaterialsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MaterialsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var materials = await _context.Materials.ToListAsync();
            return View(materials);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Material material)
        {
            if (ModelState.IsValid)
            {
                _context.Add(material);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(material);
        }

        [HttpPost]
        public async Task<IActionResult> IncreaseQuantity(int id, double quantityToAdd)
        {
            var material = await _context.Materials.FindAsync(id);
            if (material != null)
            {
                material.Quantity += quantityToAdd;
                _context.Materials.Update(material);

                var materialLog = new MaterialLog
                {
                    LogType = LogType.Add,
                    Material = material,
                    Quantity = quantityToAdd
                };
                await _context.MaterialLogs.AddAsync(materialLog);

                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task <IActionResult> Delete(int id)
        {
            var material = await _context.Materials.FindAsync(id);
            if (material != null)
            {
                _context.Materials.Remove(material);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
    }
}
