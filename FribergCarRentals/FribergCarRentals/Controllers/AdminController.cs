using FribergCarRentals.Data;
using FribergCarRentals.Models;
using Microsoft.AspNetCore.Mvc;

namespace FribergCarRentals.Controllers
{
    public class AdminController : Controller
    {
        private readonly IRepository<Admin> adminRepository;

        public AdminController(IRepository<Admin> adminRepository)
        {
            this.adminRepository = adminRepository;
        }
        [HttpGet]
        public IActionResult LogIn()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> LogIn(string email, string password)
        {
            var admin = await adminRepository.FirstOrDefaultAsync(a => a.Email == email && a.Lösenord == password);

            if (admin != null)
            {
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
