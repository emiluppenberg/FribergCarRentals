using FribergCarRentals.Data;
using FribergCarRentals.Helpers;
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
        private readonly IBilRepository bilRepository;
        private readonly IRepository<Kund> kundRepository;
        private readonly IBokningRepository bokningRepository;

        public AdminController(IRepository<Admin> adminRepository, IBilRepository bilRepository, IRepository<Kund> kundRepository, IBokningRepository bokningRepository)
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
        public async Task<IActionResult> LoggaUt(string? returnUrl)
        {
            if (!HttpContext.User.HasClaim(ClaimTypes.Role, "admin"))
            {
                if (returnUrl != null)
                {
                    return LocalRedirect(returnUrl);
                }

                return RedirectToAction("Index", "Home");
            }

            await HttpContext.SignOutAsync("MyCookie");

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

        [HttpPut]
        public async Task<IActionResult> TaBortBil(int bilId)
        {
            if (!HttpContext.User.HasClaim(ClaimTypes.Role, "admin"))
            {
                string returnUrl = HttpContext.Request.Path;
                return RedirectToAction("Index", new { returnUrl = returnUrl });
            }

            try
            {
                var bil = await bilRepository.GetByIdAsync(bilId);
                bil.ÄrAktiv = false;
                bilRepository.Update(bil);
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
        public async Task<IActionResult> ÄndraBil(int bilId)
        {
            if (!HttpContext.User.HasClaim(ClaimTypes.Role, "admin"))
            {
                string returnUrl = HttpContext.Request.Path;
                return RedirectToAction("Index", new { returnUrl = returnUrl });
            }

            var bil = await bilRepository.GetByIdAsync(bilId);

            return View(bil);
        }

        [HttpPut]
        public async Task<IActionResult> ÄndraBil([FromBody] Bil model)
        {
            if (!HttpContext.User.HasClaim(ClaimTypes.Role, "admin"))
            {
                return StatusCode(401, "Adminbehörighet krävs");
            }

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

                foreach (var bokning in kund.Bokningar)
                {
                    if (!bokning.Genomförd)
                    {
                        if (bokning.Startdatum <= DateTime.Now)
                        {
                            return StatusCode(400, "Kunden har en pågående bokning");
                        }

                        bokningRepository.Remove(bokning);
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
        public async Task<IActionResult> ÄndraBokning(int bokningId)
        {
            if (!HttpContext.User.HasClaim(ClaimTypes.Role, "admin"))
            {
                return RedirectToAction(
                    "Error",
                    "Home",
                    new
                    {
                        error = "Adminbehörighet krävs",
                        returnUrl = "/Home"
                    });
            }

            var bokning = await bokningRepository.GetByIdIncludeAsync(bokningId);

            bokning.Bil.Bilder = bokning.Bil.Bilder.ToList();

            return View(bokning);
        }

        [HttpPut]
        public async Task<IActionResult> ÄndraBokning([FromBody] Bokning model)
        {
            if (model == null || BokningarHelper.ValidateBokning(model))
            {
                return StatusCode(400, "Ogiltiga datum");
            }

            if (HttpContext.User.HasClaim(ClaimTypes.Role, "admin"))
            {
                try
                {
                    var bokning = await bokningRepository.GetByIdAsync(model.Id);

                    if (bokning.Startdatum <= DateTime.Today && bokning.Startdatum != model.Startdatum)
                    {
                        return StatusCode(400, "Startdatum går inte ändras för en pågående bokning");
                    }

                    bokning.Startdatum = model.Startdatum;
                    bokning.Slutdatum = model.Slutdatum;

                    if (!await bokningRepository.AnyBetweenDatesAsync(bokning))
                    {
                        bokningRepository.Update(bokning);

                        await bokningRepository.SaveChangesAsync();

                        return Ok(new
                        {
                            result = "Bokningen har uppdaterats",
                            startDatum = bokning.Startdatum.ToShortDateString(),
                            slutDatum = bokning.Slutdatum.ToShortDateString()
                        });
                    }
                    else
                    {
                        return StatusCode(400, "Ogiltiga datum");
                    }
                }
                catch (Exception ex)
                {
                    return StatusCode(500, ex.Message);
                }
            }

            return StatusCode(401, "Adminbehörighet krävs");
        }

        [HttpDelete]
        public async Task<IActionResult> TaBortBokning(int bokningId)
        {
            if (HttpContext.User.HasClaim(ClaimTypes.Role, "admin"))
            {
                try
                {
                    var bokning = await bokningRepository.GetByIdAsync(bokningId);

                    bokningRepository.Remove(bokning);
                    await bokningRepository.SaveChangesAsync();

                    return Ok("Bokningen har tagits bort");
                }
                catch (Exception ex)
                {
                    return StatusCode(500, ex.Message);
                }
            }

            return StatusCode(401, "Adminbehörighet krävs");
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

            return View(kund.Bokningar);
        }

        [HttpPut]
        public async Task<IActionResult> AvslutaBokning([FromBody] Bokning model)
        {
            if (!HttpContext.User.HasClaim(ClaimTypes.Role, "admin"))
            {
                return StatusCode(401, "Adminbehörighet krävs");
            }

            if (model == null)
            {
                return StatusCode(400, "Ange slutdatum");
            }

            var bokning = await bokningRepository.GetByIdAsync(model.Id);

            if (bokning.Startdatum > DateTime.Today)
            {
                return StatusCode(400, "Bokningen kan inte avslutas, startdatumet ligger framåt i tiden");
            }

            if (model.Slutdatum > DateTime.Today)
            {
                return StatusCode(400, "Bokningen kan inte avslutas, slutdatumet ligger framåt i tiden");
            }

            bokning.Slutdatum = model.Slutdatum;
            bokning.Genomförd = true;

            if (!await bokningRepository.AnyBetweenDatesAsync(bokning))
            {
                try
                {
                    bokningRepository.Update(bokning);
                    await bokningRepository.SaveChangesAsync();

                    return Ok("Bokningen har avslutats");
                }
                catch (Exception ex)
                {
                    return StatusCode(500, ex.Message);
                }
            }

            return StatusCode(400, "Bokningen kan inte avslutas, slutdatumet krockar med en annan bokning");
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

            var bilar = await bilRepository.GetAllWithKommandeBokningarAsync();

            return View(bilar);
        }

        [HttpGet]
        public async Task<IActionResult> PågåendeBokningar()
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

            var bilar = await bilRepository.GetAllWithPågåendeBokningarAsync();

            return View(bilar);
        }

        [HttpGet]
        public async Task<IActionResult> AvslutadeBokningar()
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

            var bilar = await bilRepository.GetAllWithAvslutadeBokningarAsync();

            return View(bilar);
        }
    }
}