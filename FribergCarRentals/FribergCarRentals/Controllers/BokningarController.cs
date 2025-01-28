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
        private readonly IBokningRepository bokningRepository;
        private readonly IRepository<Kund> kundRepository;

        public BokningarController(IBokningRepository bokningRepository, IRepository<Kund> kundRepository)
        {
            this.bokningRepository = bokningRepository;
            this.kundRepository = kundRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!HttpContext.User.HasClaim(ClaimTypes.Role, "kund"))
            {
                string returnUrl = HttpContext.Request.Path;
                return RedirectToAction("Index", "Kund", new { returnUrl = returnUrl });
            }

            var userId = Convert.ToInt32(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

            var bokningar = await bokningRepository.GetAllKundBokningarAsync(userId);

            return View(bokningar);
        }

        [HttpPost]
        public async Task<IActionResult> NyBokning([FromBody] Bokning model)
        {
            if (!HttpContext.User.HasClaim(ClaimTypes.Role, "kund"))
            {
                return StatusCode(401);
            }

            if (model == null || !BokningarHelper.ValidateBokning(model))
            {
                return StatusCode(400, "Ogiltiga datum");
            }

            var userId = Convert.ToInt32(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!.ToString());

            var bokning = new Bokning()
            {
                Startdatum = model.Startdatum,
                Slutdatum = model.Slutdatum,
                BilId = model.BilId,
                KundId = userId
            };

            try
            {
                if (!await bokningRepository.AnyBetweenDatesAsync(bokning))
                {
                    await bokningRepository.AddAsync(bokning);
                    await bokningRepository.SaveChangesAsync();

                    return Ok(new
                    {
                        startDatum = model.Startdatum.ToShortDateString(),
                        slutDatum = model.Slutdatum.ToShortDateString(),
                        result = "Bokningen har lagts till"
                    });
                }

                return StatusCode(400, "Ogiltiga datum");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        [HttpDelete]
        public async Task<IActionResult> TaBortBokning(int bokningId)
        {
            if (HttpContext.User.HasClaim(ClaimTypes.Role, "kund"))
            {
                var userId = Convert.ToInt32(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

                try
                {
                    var bokning = await bokningRepository.GetByIdAsync(bokningId);

                    if (bokning.Startdatum <= DateTime.Now)
                    {
                        return StatusCode(400, "Bokningen kan inte tas bort, startdatumet har passerat");
                    }

                    if (bokning.KundId != userId)
                    {
                        return StatusCode(401, "Ditt KundId matchar inte bokningens KundId");
                    }

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
            if (!HttpContext.User.HasClaim(ClaimTypes.Role, "kund"))
            {
                return RedirectToAction(
                "Error",
                "Home",
                new
                {
                    error = "Inloggning krävs",
                    returnUrl = "/Home"
                });
            }

            var userId = Convert.ToInt32(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

            var bokning = await bokningRepository.GetByIdIncludeAsync(bokningId);

            if (bokning.KundId != userId)
            {
                return RedirectToAction(
                    "Error",
                    "Home",
                    new
                    {
                        error = "Ditt KundId matchar inte bokningens KundId",
                        returnUrl = "/Bokningar"
                    });
            }

            return View(bokning);
        }

        [HttpPut]
        public async Task<IActionResult> ÄndraBokning([FromBody] Bokning model)
        {
            if (model == null || !BokningarHelper.ValidateBokning(model))
            {
                return StatusCode(400, "Ogiltiga datum");
            }

            if (HttpContext.User.HasClaim(ClaimTypes.Role, "kund"))
            {
                var userId = Convert.ToInt32(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

                try
                {
                    var bokning = await bokningRepository.GetByIdAsync(model.Id);

                    if (bokning.KundId != userId)
                    {
                        return StatusCode(401, "Ditt KundId matchar inte bokningens KundId");
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

            return StatusCode(401, "Inloggning krävs");
        }
    }
}
