using Materials.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Materials.Models;
using Microsoft.EntityFrameworkCore;

namespace Materials.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(User user)
        {
            var userCheck = await _context.Users.FirstOrDefaultAsync(u => u.UserName == user.UserName);

            if (userCheck == null)
            {
                TempData["error"] = "Incorrect email or password";
                return View("Login");
            }

            if (user.Password != userCheck.Password)
            {
                TempData["error"] = "Incorrect email or password";
                return View("Login");
            }

            var claim = Authenticate(userCheck);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claim));
            return RedirectToAction("Index", "Home");

        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        private ClaimsIdentity Authenticate(User user)
        {
            var claims = new List<Claim>()
        {
            new Claim(ClaimsIdentity.DefaultNameClaimType, user.UserName),
            new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role.Name)
        };
            return new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
        }
    }
}
