using Materials.Data;
using Materials.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Materials.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _context.Users.ToListAsync();
            return View(users);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Roles = await _context.Roles.ToListAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(User user)
        {

            ModelState.Remove("Role");

            if (ModelState.IsValid)
            {
                var usercheck = await _context.Users.AnyAsync(u => u.UserName == user.UserName);

                if (usercheck == false)
                {
                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }

                ViewBag.Roles = await _context.Roles.ToListAsync();

                return View(user);
            }

            ViewBag.Roles = await _context.Roles.ToListAsync();

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            ViewBag.Roles = await _context.Roles.ToListAsync();
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(User updatedUser)
        {
            ModelState.Remove("Role");

            if (ModelState.IsValid)
            {
                var usercheck = await _context.Users.AnyAsync(u => u.UserName == updatedUser.UserName && u.UserId != updatedUser.UserId);

                if (usercheck == false)
                {
                    _context.Users.Update(updatedUser);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }

                ViewBag.Roles = await _context.Roles.ToListAsync();

                return View(updatedUser);
            }

            ViewBag.Roles = await _context.Roles.ToListAsync();

            return View(updatedUser);
        }
    }
}
