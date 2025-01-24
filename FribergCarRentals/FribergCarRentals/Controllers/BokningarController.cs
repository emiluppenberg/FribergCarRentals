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
        private readonly IRepository<Kund> kundRepository;

        public BokningarController(IRepository<Bokning> bokningRepository, IRepository<Bil> bilRepository, IRepository<Kund> kundRepository)
        {
            this.bokningRepository = bokningRepository;
            this.bilRepository = bilRepository;
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

            var kundId = Convert.ToInt32(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

            var kund = await kundRepository.GetByIdAsync(kundId);

            return View(kund.Bokningar);
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
                BilId = model.BilId
            };

            try
            {
                var bokningar = await bokningRepository.FindAllAsync(x => x.BilId == model.BilId);

                if (BokningarHelper.ValidateNewBokning(bokning) &&
                    BokningarHelper.CheckDateAvailability(bokningar!, bokning))
                {
                    var userId = Convert.ToInt32(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!.ToString());
                    var kund = await kundRepository.GetByIdAsync(userId);
                    kund.Bokningar.Add(bokning);

                    kundRepository.Update(kund);
                    await kundRepository.SaveChangesAsync();

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
                    var kund = await kundRepository.GetByIdAsync(userId);
                    var bokning = kund.Bokningar.Find(x => x.Id == bokningId);

                    if (bokning == null)
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
            if (HttpContext.User.HasClaim(ClaimTypes.Role, "kund"))
            {
                var userId = Convert.ToInt32(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

                var kund = await kundRepository.GetByIdAsync(userId);

                var bokning = kund.Bokningar.Find(x => x.Id == bokningId);

                if (bokning == null)
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

            if (HttpContext.User.HasClaim(ClaimTypes.Role, "kund"))
            {
                var userId = Convert.ToInt32(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

                try
                {
                    var kund = await kundRepository.GetByIdAsync(userId);

                    var bokning = kund.Bokningar.Find(x => x.Id == model.Id);

                    if (bokning == null)
                    {
                        return StatusCode(401, "Ditt KundId matchar inte bokningens KundId");
                    }

                    bokning.Startdatum = model.Startdatum;
                    bokning.Slutdatum = model.Slutdatum;

                    var bokningar = await bokningRepository.FindAllAsync(x => x.BilId == bokning.BilId && x.Id != bokning.Id);

                    if (bokningar!.Count() == 0 ||
                        (BokningarHelper.ValidateNewBokning(bokning) && 
                        BokningarHelper.CheckDateAvailability(bokningar!, bokning)))
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
