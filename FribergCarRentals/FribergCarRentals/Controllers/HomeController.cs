using System.Diagnostics;
using FribergCarRentals.Data;
using FribergCarRentals.Models;
using Microsoft.AspNetCore.Mvc;

namespace FribergCarRentals.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBilRepository bilRepository;

        public HomeController(ILogger<HomeController> logger, IBilRepository bilRepository)
        {
            _logger = logger;
            this.bilRepository = bilRepository;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var bilar = new List<Bil>();

                await foreach (var bil in bilRepository.GetRandomBilarAsync()!)
                {
                    bilar.Add(bil);
                }

                return View(bilar);
            }
            catch (Exception ex)
            {
                return RedirectToAction(
                    "Error",
                    "Home",
                    new { error = ex.Message });
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(string error, string? returnUrl)
        {
            if (returnUrl == null)
            {
                returnUrl = "/Home";
            }

            ViewBag.Error = error;
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
    }
}
