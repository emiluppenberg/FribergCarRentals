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
        public async Task<IActionResult> NyBokning([FromBody] Bokning model)
        {
            if (!HttpContext.User.HasClaim(ClaimTypes.Role, "kund"))
            {
                return StatusCode(401);
            }

            if (model == null || model.Startdatum > model.Slutdatum)
            {
                return StatusCode(400, "Ogiltiga datum");
            }

            var bokning = new Bokning()
            {
                Startdatum = model.Startdatum,
                Slutdatum = model.Slutdatum,
                KundId = Convert.ToInt32(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)),
                BilId = model.BilId
            };

            var bokningar = await bokningRepository.FindAllAsync(x => x.BilId == model.BilId);

            if (bokningar != null && 
                BokningarHelper.ValidateNewBokning(bokning) &&
                BokningarHelper.CheckDateAvailability(bokningar, bokning))
            {
                try
                {
                    await bokningRepository.AddAsync(bokning);
                    await bokningRepository.SaveChangesAsync();

                    return Ok(new {
                        startDatum = model.Startdatum.ToShortDateString(),
                        slutDatum = model.Slutdatum.ToShortDateString(),
                        result = "Bokningen har lagts till"
                    });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, ex.Message);
                }
            }

            return StatusCode(400, "Ogiltiga datum");
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

            return StatusCode(401, "Inloggning krävs");
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
            if (model == null || model.Startdatum > model.Slutdatum)
            {
                return StatusCode(400, "Ogiltiga datum");
            }

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

                        var bokningar = await bokningRepository.FindAllAsync(x => x.BilId == bokning.BilId && x.Id != bokning.Id);

                        if (bokningar.Count() == 0 || 
                            (BokningarHelper.ValidateNewBokning(bokning) && BokningarHelper.CheckDateAvailability(bokningar, bokning)))
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

            return StatusCode(401, "Inloggning krävs");
        }

        [HttpPut]
        public async Task<IActionResult> GenomförBokning(int bokningId)
        {
            if (!HttpContext.User.HasClaim(ClaimTypes.Role, "admin"))
            {
                return StatusCode(401, "Adminbehörighet krävs");
            }

            var bokning = await bokningRepository.GetByIdAsync(bokningId);

            if (bokning.Startdatum > DateTime.Now)
            {
                return StatusCode(400, "Bokningen kan inte markeras som genomförd, datumen ligger framåt i tiden");
            }

            if (bokning.Slutdatum > DateTime.Now)
            {
                bokning.Slutdatum = DateTime.Now;
            }

            bokning.Genomförd = true;

            try
            {
                bokningRepository.Update(bokning);
                await bokningRepository.SaveChangesAsync();

                return Ok("Bokningen har markerats som genomförd");
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
