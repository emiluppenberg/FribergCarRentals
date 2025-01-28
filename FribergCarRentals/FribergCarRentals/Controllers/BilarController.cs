using FribergCarRentals.Data;
using FribergCarRentals.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FribergCarRentals.Controllers
{
    public class BilarController : Controller
    {
        private readonly IBilRepository bilRepository;

        public BilarController(IBilRepository bilRepository)
        {
            this.bilRepository = bilRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var bilar = await bilRepository.GetAllWithBokningarAsync();

            return View(bilar);
        }

        [HttpGet]
        public async Task<IActionResult> IndexAdmin()
        {
            if (!HttpContext.User.HasClaim(ClaimTypes.Role, "admin"))
            {
                string returnUrl = HttpContext.Request.Path;
                return RedirectToAction("Index", new { returnUrl = returnUrl });
            }

            var bilar = await bilRepository.GetAllAsync();

            return View(bilar);
        }

        [HttpGet]
        public IActionResult NyBilAdmin()
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
        public async Task<IActionResult> NyBilAdmin([FromBody] Bil model)
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

        [HttpPut]
        public async Task<IActionResult> TaBortBilAdmin(int bilId)
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

        [Route("/Bilar/ÄndraBilAdmin/{bilId}")]
        [HttpGet]
        public async Task<IActionResult> ÄndraBilAdmin(int bilId)
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
        public async Task<IActionResult> ÄndraBilAdmin([FromBody] Bil model)
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
    }
}
