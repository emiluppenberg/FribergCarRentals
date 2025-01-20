using FribergCarRentals.Data;
using FribergCarRentals.Helpers;
using FribergCarRentals.Models;
using FribergCarRentals.Viewmodels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Threading.Tasks.Dataflow;

namespace FribergCarRentals.Controllers
{
    public class BokningarController : Controller
    {
        private readonly IRepository<Bokning> bokningRepository;
        private readonly IRepository<Bil> bilRepository;

        public BokningarController(IRepository<Bokning> bokningRepository, IRepository<Bil> bilRepository)
        {
            this.bokningRepository = bokningRepository;
            this.bilRepository = bilRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!HttpContext.User.HasClaim(ClaimTypes.Role, "kund"))
            {
                string returnUrl = HttpContext.Request.Path;
                return RedirectToAction("Index", "Kund", new { returnUrl = returnUrl });
            }

            var kundId = Convert.ToInt32(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

            var bokningar = await bokningRepository.FindAllAsync(x => x.KundId == kundId);

            return View(bokningar);
        }

        [HttpPost]
        public async Task<IActionResult> NyBokning(DateTime startDatum, DateTime slutDatum, int bilId)
        {
            if (!HttpContext.User.HasClaim(ClaimTypes.Role, "kund"))
            {
                string returnUrl = "/Bilar";
                return RedirectToAction("Index", "Kund", new { returnUrl = returnUrl });
            }

            var bokning = new Bokning()
            {
                Startdatum = startDatum,
                Slutdatum = slutDatum,
                KundId = Convert.ToInt32(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)),
                BilId = bilId
            };

            var bokningar = await bokningRepository.FindAllAsync(x => x.BilId == bilId);

            if (bokningar != null && BokningarHelper.CheckDateAvailability(bokningar, bokning))
            {
                try
                {
                    await bokningRepository.AddAsync(bokning);
                    await bokningRepository.SaveChangesAsync();

                    return RedirectToAction("BokningsBekräftelse", new { bokningId = bokning.Id, bilId = bilId });
                }
                catch (Exception ex)
                {
                    return RedirectToAction(
                        "Error",
                        "Home",
                        new
                        {
                            error = ex.Message,
                            returnUrl = "/Bilar"
                        });
                }
            }

            return RedirectToAction(
                    "Error",
                    "Home",
                    new
                    {
                        error = "Bilen är redan uppbokad dessa datum",
                        returnUrl = "/Bilar"
                    });
        }

        [HttpGet]
        public async Task<IActionResult> BokningsBekräftelse(int bokningId, int bilId)
        {
            var bokning = await bokningRepository.FirstOrDefaultAsync(x => x.Id == bokningId);

            if (bokning != null)
            {
                return View(bokning);
            }

            return RedirectToAction(
                "Error",
                "Home",
                new
                {
                    error = "Bokningen kunde inte bekräftas",
                    returnUrl = "/Bilar"
                });
        }

        [HttpDelete]
        public async Task<IActionResult> TaBortBokning(int bokningId)
        {
            if (HttpContext.User.HasClaim(ClaimTypes.Role, "kund") ||
                HttpContext.User.HasClaim(ClaimTypes.Role, "admin"))
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

            return RedirectToAction(
                "Error",
                "Home",
                new
                {
                    error = "Inloggning krävs",
                    returnUrl = "/Home"
                });
        }

        [HttpGet]
        public async Task<IActionResult> ÄndraBokning(int bokningId)
        {
            if (HttpContext.User.HasClaim(ClaimTypes.Role, "kund") || HttpContext.User.HasClaim(ClaimTypes.Role, "admin"))
            {
                var bokning = await bokningRepository.FirstOrDefaultAsync(x => x.Id == bokningId);

                if (bokning == null)
                {
                    return RedirectToAction(
                        "Error",
                        "Home",
                        new
                        {
                            error = "Bokningen hittades inte i databasen",
                            returnUrl = "/Bokningar"
                        });
                }

                var userId = Convert.ToInt32(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

                if (userId == bokning.KundId || HttpContext.User.HasClaim(ClaimTypes.Role, "admin"))
                {
                    return View(bokning);
                }
            }

            return RedirectToAction(
                "Error",
                "Home",
                new
                {
                    error = "Inloggning krävs",
                    returnUrl = "/Home"
                });
        }

        [HttpPut]
        public async Task<IActionResult> ÄndraBokning([FromBody] Bokning model)
        {
            if (HttpContext.User.HasClaim(ClaimTypes.Role, "kund") || HttpContext.User.HasClaim(ClaimTypes.Role, "admin"))
            {
                var userId = Convert.ToInt32(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

                try
                {
                    var bokning = await bokningRepository.GetByIdAsync(model.Id);

                    if (userId == bokning.KundId || HttpContext.User.HasClaim(ClaimTypes.Role, "admin"))
                    {
                        bokning.Startdatum = model.Startdatum;
                        bokning.Slutdatum = model.Slutdatum;

                        var bokningar = await bokningRepository.FindAllAsync(x => x.BilId == bokning.BilId);

                        if (BokningarHelper.CheckDateAvailability(bokningar, bokning))
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
                            return StatusCode(400, "Bilen är redan uppbokad dessa datum");
                        }
                    }
                    else
                    {
                        return StatusCode(401, "Ditt KundId matchar inte bokningens KundId");
                    }
                }
                catch (Exception ex)
                {
                    return StatusCode(500, ex.Message);
                }
            }

            return RedirectToAction(
                "Error",
                "Home",
                new
                {
                    error = "Inloggning krävs",
                    returnUrl = "/Home"
                });
        }
    }
}
