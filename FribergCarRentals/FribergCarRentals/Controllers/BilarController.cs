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
        public async Task<IActionResult> Index()
        {
            var bilar = await bilRepository.GetAllWithBokningarAsync();

            return View(bilar);
        }
    }
}
