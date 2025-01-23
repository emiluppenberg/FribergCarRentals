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
        private readonly IRepository<Kund> kundRepository;
        private readonly IRepository<Bokning> bokningRepository;

        public AdminController(IRepository<Admin> adminRepository, IRepository<Bil> bilRepository, IRepository<Kund> kundRepository, IRepository<Bokning> bokningRepository)
        {
            this.adminRepository = adminRepository;
            this.bilRepository = bilRepository;
            this.kundRepository = kundRepository;
            this.bokningRepository = bokningRepository;
        }

        [HttpGet]
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

                if (returnUrl != null)
                {
                    return LocalRedirect(returnUrl);
                }
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

                return StatusCode(400, new { errors = errors });
            }

            var bilder = new List<string>();

            foreach (var bild in model.Bilder)
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
                return Ok("Bilen har lagts till!");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
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
        public async Task<IActionResult> TaBortBil(int bilId)
        {
            if (!HttpContext.User.HasClaim(ClaimTypes.Role, "admin"))
            {
                string returnUrl = HttpContext.Request.Path;
                return RedirectToAction("Index", new { returnUrl = returnUrl });
            }

            try
            {
                bilId = 37;
                var bil = await bilRepository.GetByIdAsync(bilId);
                bilRepository.Remove(bil);
                await bilRepository.SaveChangesAsync();
                return Ok("Bilen har tagits bort");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Route("/Admin/ÄndraBil/{bilId}")]
        [HttpGet]
        public IActionResult ÄndraBil(int bilId)
        {
            if (!HttpContext.User.HasClaim(ClaimTypes.Role, "admin"))
            {
                string returnUrl = HttpContext.Request.Path;
                return RedirectToAction("Index", new { returnUrl = returnUrl });
            }

            var bil = bilRepository.GetById(bilId);

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

                return StatusCode(400, new { errors = errors });
            }

            var bilder = new List<string>();

            foreach (var bild in model.Bilder)
            {
                if (bild != string.Empty)
                {
                    bilder.Add(bild);
                }
            }

            try
            {
                model.Id = 37;
                var bil = await bilRepository.GetByIdAsync(model.Id);

                bil.Tillverkare = model.Tillverkare;
                bil.Årsmodell = model.Årsmodell;
                bil.Modell = model.Modell;
                bil.Bränsle = model.Bränsle;
                bil.Växellåda = model.Växellåda;
                bil.Drivning = model.Drivning;
                bil.Beskrivning = model.Beskrivning;
                bil.Bilder = bilder;

                bilRepository.Update(bil);
                await bilRepository.SaveChangesAsync();
                return Ok("Bilen har ändrats");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Kunder()
        {
            if (!HttpContext.User.HasClaim(ClaimTypes.Role, "admin"))
            {
                return RedirectToAction(
                    "Error",
                    "Home",
                    new
                    {
                        error = "Adminbehörighet krävs"
                    });
            }

            var kunder = await kundRepository.GetAllAsync();

            return View(kunder);
        }

        [HttpGet]
        public async Task<IActionResult> ÄndraKund(int kundId)
        {
            if (!HttpContext.User.HasClaim(ClaimTypes.Role, "admin"))
            {
                return RedirectToAction(
                    "Error",
                    "Home",
                    new
                    {
                        error = "Adminbehörighet krävs"
                    });
            }

            var kund = await kundRepository.GetByIdAsync(kundId);

            return View(kund);
        }

        [HttpPost]
        public async Task<IActionResult> ÄndraKund(Kund kund)
        {
            if (!ModelState.IsValid)
            {
                return View(kund);
            }

            try
            {
                kundRepository.Update(kund);
                await kundRepository.SaveChangesAsync();
                ViewBag.Result = "Ändringar sparade";
                return View();
            }
            catch (Exception ex)
            {
                string returnUrl = HttpContext.Request.Path;

                return RedirectToAction(
                    "Error",
                    "Home",
                    new
                    {
                        error = ex.Message,
                        returnUrl = returnUrl
                    });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> TaBortKund(int kundId)
        {
            if (!HttpContext.User.HasClaim(ClaimTypes.Role, "admin"))
            {
                return RedirectToAction(
                    "Error",
                    "Home",
                    new
                    {
                        error = "Adminbehörighet krävs"
                    });
            }

            try
            {
                var kund = await kundRepository.GetByIdAsync(kundId);

                var bokningar = await bokningRepository.FindAllAsync(x => x.KundId == kund.Id);

                if (bokningar!.Count() != 0)
                {
                    foreach (var bokning in bokningar!)
                    {
                        if (!bokning.Genomförd)
                        {
                            bokningRepository.Remove(bokning);
                        }
                        else
                        {
                            bokning.KundId = 0;
                            bokningRepository.Update(bokning);
                        }
                    }
                }

                await bokningRepository.SaveChangesAsync();

                kundRepository.Remove(kund);

                await kundRepository.SaveChangesAsync();

                return Ok("Kunden har tagits bort");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> KundBokningar(int kundId)
        {
            if (!HttpContext.User.HasClaim(ClaimTypes.Role, "admin"))
            {
                return RedirectToAction(
                    "Error",
                    "Home",
                    new
                    {
                        error = "Adminbehörighet krävs",
                    });
            }

            var kund = await kundRepository.GetByIdAsync(kundId);

            ViewBag.KundEmail = kund.Email;

            var bokningar = await bokningRepository.FindAllAsync(x => x.KundId == kund.Id);

            return View(bokningar);
        }

        [HttpGet]
        public async Task<IActionResult> KommandeBokningar()
        {
            if (!HttpContext.User.HasClaim(ClaimTypes.Role, "admin"))
            {
                return RedirectToAction(
                    "Error",
                    "Home",
                    new
                    {
                        error = "Adminbehörighet krävs"
                    });
            }

            var bokadeBilar = await bilRepository.FindAllAsync(x => x.Bokningar!.Count > 0);

            var kommandeBokadeBilar = bokadeBilar!.ToList();

            for (int i = 0; i < kommandeBokadeBilar.Count; i++)
            {
                kommandeBokadeBilar[i].Bokningar!.RemoveAll(x => x.Genomförd == true);

                if (kommandeBokadeBilar[i].Bokningar!.Count() == 0)
                {
                    kommandeBokadeBilar.Remove(kommandeBokadeBilar[i]);
                    i--;
                }
            }

            return View(kommandeBokadeBilar);
        }

        [HttpGet]
        public async Task<IActionResult> GenomfördaBokningar()
        {
            if (!HttpContext.User.HasClaim(ClaimTypes.Role, "admin"))
            {
                return RedirectToAction(
                    "Error",
                    "Home",
                    new
                    {
                        error = "Adminbehörighet krävs"
                    });
            }

            var bokadeBilar = await bilRepository.FindAllAsync(x => x.Bokningar!.Count > 0);

            var genomfördaBokadeBilar = bokadeBilar!.ToList();

            for (int i = 0; i < genomfördaBokadeBilar.Count; i++)
            {
                genomfördaBokadeBilar[i].Bokningar!.RemoveAll(x => x.Genomförd == false);

                if (genomfördaBokadeBilar[i].Bokningar!.Count() == 0)
                {
                    genomfördaBokadeBilar.Remove(genomfördaBokadeBilar[i]);
                    i--;
                }
            }

            return View(genomfördaBokadeBilar);
        }
    }
}