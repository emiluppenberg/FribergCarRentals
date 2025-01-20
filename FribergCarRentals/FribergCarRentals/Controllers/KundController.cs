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

        public IActionResult Index(string? returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        public IActionResult Registrera(string? returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoggaIn(string email, string password, string? returnUrl)
        {
            var kund = await kundRepository.FirstOrDefaultAsync(k => k.Email == email && k.Lösenord == password);

            if (kund != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Role, "kund"),
                    new Claim(ClaimTypes.NameIdentifier, kund.Id.ToString()),
                    new Claim(ClaimTypes.Name, kund.Email)
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync("MyCookie", principal);

                if (returnUrl != null)
                {
                    return LocalRedirect(returnUrl);
                }
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Registrera(Kund kund, string? returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(kund);
            }

            if (await kundRepository.FirstOrDefaultAsync(x => x.Email == kund.Email) != null)
            {
                ModelState.AddModelError("Email", "Det finns redan en kund med denna email adress");
                return View(kund);
            }

            await kundRepository.AddAsync(kund);
            await kundRepository.SaveChangesAsync();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Role, "kund"),
                new Claim(ClaimTypes.NameIdentifier, kund.Id.ToString()),
                new Claim(ClaimTypes.Name, kund.Email)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("MyCookie", principal);

            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> MinaUppgifter()
        {
            if (!HttpContext.User.HasClaim(ClaimTypes.Role, "kund"))
            {
                string returnUrl = HttpContext.Request.Path;

                return RedirectToAction(
                    "Error",
                    "Home",
                    new
                    {
                        error = "Inloggning krävs",
                        returnUrl = returnUrl
                    });
            }

            var kundId = Convert.ToInt32(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

            var kund = await kundRepository.GetByIdAsync(kundId);

            return View(kund);
        }
    }
}
