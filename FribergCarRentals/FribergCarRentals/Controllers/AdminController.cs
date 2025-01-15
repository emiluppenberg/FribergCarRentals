using FribergCarRentals.Data;
using FribergCarRentals.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Abstractions;
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

        public IActionResult Index(string? returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoggaIn(string email, string password, string? returnUrl)
        {
            var admin = await adminRepository.FirstOrDefaultAsync(a => a.Email == email && a.Lösenord == password);

            if (admin != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Role, "admin"),
                    new Claim(ClaimTypes.NameIdentifier, admin.Id.ToString()),
                    new Claim(ClaimTypes.Name, admin.Email)
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync("MyCookie", principal);

                return LocalRedirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult NyBil()
        {
            if (!HttpContext.User.HasClaim(ClaimTypes.Role, "admin"))
            {
                string returnUrl = HttpContext.Request.Path;
                return RedirectToAction("Index", new { returnUrl = returnUrl });
            }

            var model = new Bil();
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> NyBil([FromBody] Bil model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(m => m.Value.Errors.Any())
                    .ToDictionary(
                        m => m.Key,
                        m => m.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                return Json(new { success = false, errors });
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
                Årsmodell = model.Årsmodell,
                Modell = model.Modell,
                Drivning = model.Drivning,
                Bränsle = model.Bränsle,
                Växellåda = model.Växellåda,
                Beskrivning = model.Beskrivning,
                Bilder = bilder
            };

            try
            {
                await bilRepository.AddAsync(bil);
                await bilRepository.SaveChangesAsync();
                return Json(new { success = true, result = "Bilen har lagts till!" });
            }
            catch(Exception ex)
            {
                return Json(new { success = true, result = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Bilar()
        {
            if (!HttpContext.User.HasClaim(ClaimTypes.Role, "admin"))
            {
                string returnUrl = HttpContext.Request.Path;
                return RedirectToAction("Index", new { returnUrl = returnUrl });
            }

            var bilar = await bilRepository.GetAllAsync();

            return View(bilar);
        }

        [HttpDelete]
        public async Task<IActionResult> TaBortBil(int id)
        {
            if (!HttpContext.User.HasClaim(ClaimTypes.Role, "admin"))
            {
                string returnUrl = HttpContext.Request.Path;
                return RedirectToAction("Index", new { returnUrl = returnUrl });
            }

            try
            {
                var bil = await bilRepository.GetByIdAsync(id);

                if (bil == null)
                {
                    return Json(new { success = false, result = "Bilen kunde inte hittas i databasen" });
                }

                bilRepository.Remove(bil);
                await bilRepository.SaveChangesAsync();
                return Json(new { success = true, result = "Bilen har tagits bort" });
            }
            catch(Exception ex)
            {
                return RedirectToAction("Error", "Home", ex);
            }
        }

        [HttpGet]
        public IActionResult ÄndraBil(int id)
        {
            if (!HttpContext.User.HasClaim(ClaimTypes.Role, "admin"))
            {
                string returnUrl = HttpContext.Request.Path;
                return RedirectToAction("Index", new { returnUrl = returnUrl });
            }

            var bil = bilRepository.GetById(id);
            return View(bil);
        }
        [HttpPost]
        public async Task<IActionResult> ÄndraBil([FromBody] Bil model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(m => m.Value.Errors.Any())
                    .ToDictionary(
                        m => m.Key,
                        m => m.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                return Json(new { success = false, errors });
            }

            var bilder = new List<string>();

            foreach (var bild in model.Bilder)
            {
                if (bild != string.Empty)
                {
                    bilder.Add(bild);
                }
            }

            var bil = await bilRepository.GetByIdAsync(model.Id);

            if (bil == null)
            {
                return Json(new { success = false, result = "Bilen kunde inte hittas i databasen" });
            }

            bil.Tillverkare = model.Tillverkare;
            bil.Årsmodell = model.Årsmodell;
            bil.Modell = model.Modell;
            bil.Bränsle = model.Bränsle;
            bil.Växellåda = model.Växellåda;
            bil.Drivning = model.Drivning;
            bil.Beskrivning = model.Beskrivning;
            bil.Bilder = bilder;

            try
            {
                bilRepository.Update(bil);
                await bilRepository.SaveChangesAsync();
                return Json(new { success = true, result = "Bilen har ändrats" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, exception = ex.Message });
            }
        }
    }
}