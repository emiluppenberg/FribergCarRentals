using FribergCarRentals.Data;
using FribergCarRentals.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FribergCarRentals.Controllers
{
    public class KundController : Controller
    {
        private readonly IRepository<Kund> kundRepository;

        public KundController(IRepository<Kund> kundRepository)
        {
            this.kundRepository = kundRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult LogIn()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> LogIn(string email, string password)
        {
            var kund = await kundRepository.FirstOrDefaultAsync(k => k.Email == email && k.Lösenord == password);

            if (kund != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Role, "kund"),
                    new Claim(ClaimTypes.NameIdentifier, kund.Email)
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync("MyCookie", principal);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
