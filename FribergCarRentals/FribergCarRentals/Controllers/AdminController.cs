using FribergCarRentals.Data;
using FribergCarRentals.Helpers;
using FribergCarRentals.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Abstractions;
using System.Security.Claims;

namespace FribergCarRentals.Controllers
{
    public class AdminController : Controller
    {
        private readonly IRepository<Admin> adminRepository;
        private readonly IBilRepository bilRepository;
        private readonly IRepository<Kund> kundRepository;
        private readonly IBokningRepository bokningRepository;
        private readonly IUserService userService;

        public AdminController(IRepository<Admin> adminRepository, IBilRepository bilRepository, IRepository<Kund> kundRepository, IBokningRepository bokningRepository, IUserService userService)
        {
            this.adminRepository = adminRepository;
            this.bilRepository = bilRepository;
            this.kundRepository = kundRepository;
            this.bokningRepository = bokningRepository;
            this.userService = userService;
        }

        [HttpGet]
        public IActionResult Index(string? returnUrl, string? result)
        {
            if (result != null)
            {
                ViewBag.Result = result;
            }

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoggaIn(string email, string password, string? returnUrl)
        {
            try
            {
                var admin = await adminRepository.FirstOrDefaultAsync(a => a.Email == email && a.Lösenord == password);

                if (admin != null)
                {
                    await userService.SignInAsync(admin);

                    if (returnUrl != null)
                    {
                        return LocalRedirect(returnUrl);
                    }

                    return RedirectToAction("Index", "Home");
                }

                string result = "Felaktigt email eller lösenord";
                return RedirectToAction("Index", new { returnUrl = returnUrl, result = result });
            }
            catch (Exception ex)
            {
                return RedirectToAction(
                    "Error",
                    "Home",
                    new { error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> LoggaUt(string? returnUrl)
        {
            await userService.SignOutAsync();

            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}