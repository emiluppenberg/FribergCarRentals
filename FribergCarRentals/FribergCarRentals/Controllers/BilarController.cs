using FribergCarRentals.Data;
using FribergCarRentals.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using System.Security.Claims;

namespace FribergCarRentals.Controllers
{
    public class BilarController : Controller
    {
        private readonly IBilRepository bilRepository;
        private readonly IUserService userService;

        public BilarController(IBilRepository bilRepository, IUserService userService)
        {
            this.bilRepository = bilRepository;
            this.userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var bilar = await bilRepository.GetAllWithBokningarAsync();

                return View(bilar);
            }
            catch (Exception ex)
            {
                return RedirectToAction(
                        "Error",
                        "Home",
                        new
                        {
                            error = ex.Message
                        });
            }
        }

        [HttpGet]
        public async Task<IActionResult> IndexAdmin()
        {
            try
            {
                userService.GetAdminId();
                var bilar = await bilRepository.GetAllAsync();

                return View(bilar);
            }
            catch (Exception ex)
            {
                return RedirectToAction(
                        "Error",
                        "Home",
                        new
                        {
                            error = ex.Message
                        });
            }


        }

        [HttpGet]
        public IActionResult NyBilAdmin()
        {
            try
            {
                userService.GetAdminId();
                var model = new Bil();

                return View(model);
            }
            catch (Exception ex)
            {
                return RedirectToAction(
                        "Error",
                        "Home",
                        new
                        {
                            error = ex.Message
                        });
            }
        }

        [HttpPost]
        public async Task<IActionResult> NyBilAdmin([FromBody] Bil model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(m => m.Value != null && m.Value.Errors.Any())
                    .ToDictionary(
                        m => m.Key,
                        m => m.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                return StatusCode(400, new { errors = errors });
            }

            try
            {
                userService.GetAdminId();

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
            try
            {
                userService.GetAdminId();
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
            try
            {
                userService.GetAdminId();
                var bil = await bilRepository.GetByIdAsync(bilId);

                return View(bil);
            }
            catch (Exception ex)
            {
                return RedirectToAction(
                        "Error",
                        "Home",
                        new
                        {
                            error = ex.Message
                        });
            }
        }

        [HttpPut]
        public async Task<IActionResult> ÄndraBilAdmin([FromBody] Bil model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(m => m.Value != null && m.Value.Errors.Any())
                    .ToDictionary(
                        m => m.Key,
                        m => m.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                return StatusCode(400, new { errors = errors });
            }

            try
            {
                userService.GetAdminId();

                var bilder = new List<string>();

                foreach (var bild in model.Bilder)
                {
                    if (bild != string.Empty)
                    {
                        bilder.Add(bild);
                    }
                }

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
