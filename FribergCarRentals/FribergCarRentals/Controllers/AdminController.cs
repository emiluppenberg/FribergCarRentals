using FribergCarRentals.Data;
using FribergCarRentals.Models;
using FribergCarRentals.Models.Viewmodels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FribergCarRentals.Controllers
{
    public class AdminController : Controller
    {
        private readonly IRepository<Admin> adminRepository;
        private readonly IRepository<Bil> bilRepository;

        public AdminController(IRepository<Admin> adminRepository, IRepository<Bil> bilRepository)
        {
            this.adminRepository = adminRepository;
            this.bilRepository = bilRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LogIn(string email, string password)
        {
            var admin = await adminRepository.FirstOrDefaultAsync(a => a.Email == email && a.Lösenord == password);

            if (admin != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Role, "admin"),
                    new Claim(ClaimTypes.NameIdentifier, admin.Email),
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync("MyCookie", principal);
            }

            return RedirectToAction("Index", "Home");
        }

        public IActionResult NyBil()
        {
            var model = new NyBilViewmodel();
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> NyBil([FromBody] NyBilViewmodel model)
        {
            if (!ModelState.IsValid)
            {
                var errorProperties = ModelState.Values
                    .SelectMany(x => x.Errors)
                    .Select(x => x.ErrorMessage)
                    .ToList();

                return BadRequest(new { ErrorProperties = errorProperties });
            }

            var bilder = new List<string>();

            foreach(var bild in model.Bilder)
            {
                if (bild != string.Empty)
                {
                    bilder.Add(bild);
                }
            }

            var bil = new Bil()
            {
                Tillverkare = model.Tillverkare,
                ÅrsModell = model.ÅrsModell,
                Modell = model.Modell,
                Drivlina = model.Drivlina,
                Bränsle = model.Bränsle,
                BränsleFörbrukning = model.BränsleFörbrukning,
                Tankvolym = model.Tankvolym,
                MaxMotoreffekt = model.MaxMotoreffekt,
                Beskrivning = model.Beskrivning,
                Bilder = bilder
            };

            try
            {
                await bilRepository.AddAsync(bil);
                await bilRepository.SaveChangesAsync();
                return Json(new { result = "Bilen har lagts till!" });
            }
            catch(Exception ex)
            {
                return Json(new { result = ex.Message });
            }
        }
    }
}
