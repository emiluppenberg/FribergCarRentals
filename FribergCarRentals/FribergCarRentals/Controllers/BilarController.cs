using FribergCarRentals.Data;
using FribergCarRentals.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FribergCarRentals.Controllers
{
    public class BilarController : Controller
    {
        private readonly IRepository<Bil> bilRepository;

        public BilarController(IRepository<Bil> bilRepository)
        {
            this.bilRepository = bilRepository;
        }
        public async Task<IActionResult> Index()
        {
            var bilar = await bilRepository.GetAllAsync();

            foreach (var bil in bilar!)
            {
                bil.Bokningar!.RemoveAll(x => x.Genomförd == true);
            }

            return View(bilar);
        }
    }
}
