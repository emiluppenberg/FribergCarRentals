using FribergCarRentals.Data;
using FribergCarRentals.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FribergCarRentals.Controllers
{
    public class BokningarController : Controller
    {
        private readonly IRepository<Bokning> bokningRepository;

        public BokningarController(IRepository<Bokning> bokningRepository)
        {
            this.bokningRepository = bokningRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> BokaBil(DateTime startDatum, DateTime slutDatum, int bilId)
        {
            if (!HttpContext.User.HasClaim(ClaimTypes.Role, "kund"))
            {
                string returnUrl = HttpContext.Request.Path;
                return RedirectToAction("Index", "Kund", new { returnUrl = returnUrl });
            }

            var bokning = new Bokning()
            {
                Startdatum = startDatum,
                Slutdatum = slutDatum,
                KundId = Convert.ToInt32(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)),
                BilId = bilId
            };

            try
            {
                await bokningRepository.AddAsync(bokning);
                await bokningRepository.SaveChangesAsync();

                return Json(new
                {
                    result = "Bokning genomförd",
                    startDatum = startDatum.ToShortDateString(),
                    slutDatum = slutDatum.ToShortDateString()
                });
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new { exception = ex.Message });
            }
        }
    }
}
