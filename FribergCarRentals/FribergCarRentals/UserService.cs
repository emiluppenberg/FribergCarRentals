using FribergCarRentals.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Security.Claims;

namespace FribergCarRentals
{
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public UserService(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task SignInAsync(User user)
        {
            var httpContext = httpContextAccessor.HttpContext ??
                throw new NullReferenceException(nameof(httpContextAccessor.HttpContext));

            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Role, user.Role),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Email)
                };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await httpContext.SignInAsync("MyCookie", principal, new AuthenticationProperties
            {
                IsPersistent = false,
                AllowRefresh = false
            });
        }

        public async Task SignOutAsync()
        {
            var httpContext = httpContextAccessor.HttpContext ??
                throw new NullReferenceException(nameof(httpContextAccessor.HttpContext));

            if (httpContext.User.Claims.Any())
            {
                await httpContext.SignOutAsync("MyCookie");
            }
        }

        public int GetKundId()
        {
            var httpContext = httpContextAccessor.HttpContext ??
                throw new NullReferenceException(nameof(httpContextAccessor.HttpContext));

            if (httpContext.User.HasClaim(ClaimTypes.Role, "kund"))
            {
                var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (userId == null)
                {
                    throw new Exception("Inloggning krävs");
                }

                return Convert.ToInt32(userId);
            }

            throw new Exception("Denna funktion är endast för kunder");
        }

        public int GetAdminId()
        {
            var httpContext = httpContextAccessor.HttpContext ??
                throw new NullReferenceException(nameof(httpContextAccessor.HttpContext));

            if (httpContext.User.HasClaim(ClaimTypes.Role, "admin"))
            {
                var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (userId == null)
                {
                    throw new Exception("Adminbehörighet krävs");
                }

                return Convert.ToInt32(userId);
            }

            throw new Exception("Denna funktion är endast för admins");
        }
    }
}
