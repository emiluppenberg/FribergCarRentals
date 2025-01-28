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

        public IActionResult Index()
        {
            var bilar = bilRepository.GetRandomBilar();

            return View(bilar);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(string error, string returnUrl)
        {
            ViewBag.Error = error;
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
    }
}
