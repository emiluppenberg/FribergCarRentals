using FribergCarRentals.Data;
using FribergCarRentals.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FribergCarRentals.Controllers
{
    public class KundController : Controller
    {
        private readonly IRepository<Kund> kundRepository;
        private readonly IBokningRepository bokningRepository;
        private readonly IUserService userService;

        public KundController(IRepository<Kund> kundRepository, IBokningRepository bokningRepository, IUserService userService)
        {
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

        [HttpGet]
        public IActionResult Registrera(string? returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoggaIn(string email, string password, string? returnUrl)
        {
            try
            {
                var kund = await kundRepository.FirstOrDefaultAsync(k => k.Email == email && k.Lösenord == password);

                if (kund != null)
                {
                    await userService.SignInAsync(kund);

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

        [HttpPost]
        public async Task<IActionResult> Registrera(Kund kund, string? returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(kund);
            }


            try
            {
                if (await kundRepository.FirstOrDefaultAsync(x => x.Email == kund.Email) != null)
                {
                    ModelState.AddModelError("Email", "Det finns redan en kund med denna email adress");
                    return View(kund);
                }

                await kundRepository.AddAsync(kund);
                await kundRepository.SaveChangesAsync();
                await userService.SignInAsync(kund);

                if (returnUrl != null)
                {
                    return LocalRedirect(returnUrl);
                }

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                return RedirectToAction(
                    "Home",
                    "Error",
                    new
                    {
                        error = ex.Message
                    });
            }
        }

        [HttpGet]
        public async Task<IActionResult> MinaUppgifter()
        {
            try
            {
                var kundId = userService.GetKundId();
                var kund = await kundRepository.GetByIdAsync(kundId);
                return View(kund);
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
        public async Task<IActionResult> KunderAdmin()
        {
            try
            {
                userService.GetAdminId();
                var kunder = await kundRepository.GetAllAsync();
                return View(kunder);
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
        public async Task<IActionResult> ÄndraKundAdmin(int kundId)
        {
            try
            {
                userService.GetAdminId();
                var kund = await kundRepository.GetByIdAsync(kundId);
                return View(kund);
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
        public async Task<IActionResult> ÄndraKundAdmin(Kund kund)
        {
            if (!ModelState.IsValid)
            {
                return View(kund);
            }

            try
            {
                userService.GetAdminId();
                kundRepository.Update(kund);
                await kundRepository.SaveChangesAsync();
                ViewBag.Result = "Ändringar sparade";
                return View();
            }
            catch (Exception ex)
            {
                string returnUrl = HttpContext.Request.Path;

                return RedirectToAction(
                    "Error",
                    "Home",
                    new
                    {
                        error = ex.Message,
                        returnUrl = returnUrl
                    });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> TaBortKundAdmin(int kundId)
        {
            try
            {
                userService.GetAdminId();
                var kund = await kundRepository.GetByIdAsync(kundId);

                foreach (var bokning in kund.Bokningar)
                {
                    if (!bokning.Genomförd)
                    {
                        if (bokning.Startdatum <= DateTime.Now)
                        {
                            return StatusCode(400, "Kunden har en pågående bokning");
                        }

                        bokningRepository.Remove(bokning);
                    }
                }

                await bokningRepository.SaveChangesAsync();

                kundRepository.Remove(kund);

                await kundRepository.SaveChangesAsync();

                return Ok("Kunden har tagits bort");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
