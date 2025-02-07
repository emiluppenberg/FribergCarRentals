using FribergCarRentals.Data;
using FribergCarRentals.Helpers;
using FribergCarRentals.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Threading.Tasks.Dataflow;

namespace FribergCarRentals.Controllers
{
    public class BokningarController : Controller
    {
        private readonly IBokningRepository bokningRepository;
        private readonly IBilRepository bilRepository;
        private readonly IRepository<Kund> kundRepository;
        private readonly IUserService userService;

        public BokningarController(IBokningRepository bokningRepository, IBilRepository bilRepository, IRepository<Kund> kundRepository, IUserService userService)
        {
            this.bokningRepository = bokningRepository;
            this.bilRepository = bilRepository;
            this.kundRepository = kundRepository;
            this.userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var kundId = userService.GetKundId();
                var bokningar = await bokningRepository.GetAllKundBokningarAsync(kundId);
                return View(bokningar);
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
        public async Task<IActionResult> NyBokning([FromBody] Bokning model)
        {
            if (model == null)
            {
                return StatusCode(400, "Fyll i datum");
            }

            var validation = BokningarHelper.ValidateNewBokning(model);

            if (validation != null)
            {
                return StatusCode(400, validation);
            }

            try
            {
                var kundId = userService.GetKundId();

                var bokning = new Bokning()
                {
                    Startdatum = model.Startdatum,
                    Slutdatum = model.Slutdatum,
                    BilId = model.BilId,
                    KundId = kundId
                };

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
            try
            {
                var kundId = userService.GetKundId();

                var bokning = await bokningRepository.GetByIdAsync(bokningId);

                if (bokning.Startdatum <= DateTime.Now)
                {
                    return StatusCode(400, "Bokningen kan inte tas bort, startdatumet har passerat");
                }

                if (bokning.KundId != kundId)
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

        [HttpGet]
        public async Task<IActionResult> ÄndraBokning(int bokningId)
        {
            try
            {
                var kundId = userService.GetKundId();

                var bokning = await bokningRepository.GetByIdIncludeAsync(bokningId);

                if (bokning.KundId != kundId)
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
        public async Task<IActionResult> ÄndraBokning([FromBody] Bokning model)
        {
            if (model == null)
            {
                return StatusCode(400, "Ogiltiga datum");
            }

            try
            {
                var kundId = userService.GetKundId();
                var bokning = await bokningRepository.GetByIdAsync(model.Id);

                if (bokning.KundId != kundId)
                {
                    return StatusCode(401, "Ditt KundId matchar inte bokningens KundId");
                }

                var validation = BokningarHelper.ValidateExistingBokning(bokning, model);

                if (validation != null)
                {
                    return StatusCode(400, validation);
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

        [HttpGet]
        public async Task<IActionResult> ÄndraBokningAdmin(int bokningId)
        {
            try
            {
                userService.GetAdminId();

                var bokning = await bokningRepository.GetByIdIncludeAsync(bokningId);

                bokning.Bil.Bilder = bokning.Bil.Bilder.ToList();

                return View(bokning);

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
        public async Task<IActionResult> ÄndraBokningAdmin([FromBody] Bokning model)
        {
            if (model == null)
            {
                return StatusCode(400, "Fyll i datum");
            }

            try
            {
                userService.GetAdminId();

                var bokning = await bokningRepository.GetByIdAsync(model.Id);

                var validation = BokningarHelper.ValidateExistingBokning(bokning, model);

                if (validation != null)
                {
                    return StatusCode(400, validation);
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

        [HttpDelete]
        public async Task<IActionResult> TaBortBokningAdmin(int bokningId)
        {
            try
            {
                userService.GetAdminId();
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

        [HttpGet]
        public async Task<IActionResult> KundBokningarAdmin(int kundId)
        {
            try
            {
                userService.GetAdminId();
                var kund = await kundRepository.GetByIdAsync(kundId);

                ViewBag.KundEmail = kund.Email;

                return View(kund.Bokningar);
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
        public async Task<IActionResult> AvslutaBokningAdmin([FromBody] Bokning model)
        {
            if (model == null)
            {
                return StatusCode(400, "Ange slutdatum");
            }

            try
            {
                userService.GetAdminId();

                var bokning = await bokningRepository.GetByIdAsync(model.Id);

                var validation = BokningarHelper.ValidateAvslutaBokning(bokning, model);

                if (validation != null)
                {
                    return StatusCode(400, validation);
                }

                bokning.Slutdatum = model.Slutdatum;
                bokning.Genomförd = true;

                if (!await bokningRepository.AnyBetweenDatesAsync(bokning))
                {
                    bokningRepository.Update(bokning);
                    await bokningRepository.SaveChangesAsync();

                    return Ok("Bokningen har avslutats");
                }

                return StatusCode(400, "Bokningen kan inte avslutas, slutdatumet krockar med en annan bokning");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> KommandeBokningarAdmin()
        {
            try
            {
                userService.GetAdminId();
                var bilar = await bilRepository.GetAllWithKommandeBokningarAsync();
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
        public async Task<IActionResult> PågåendeBokningarAdmin()
        {
            try
            {
                userService.GetAdminId();
                var bilar = await bilRepository.GetAllWithPågåendeBokningarAsync();

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
        public async Task<IActionResult> AvslutadeBokningarAdmin()
        {
            try
            {
                userService.GetAdminId();
                var bilar = await bilRepository.GetAllWithAvslutadeBokningarAsync();

                return View(bilar);
            }
            catch(Exception ex)
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
    }
}
