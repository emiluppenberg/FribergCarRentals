using FribergCarRentals.Models;

namespace FribergCarRentals
{
    public interface IUserService
    {
        Task SignInAsync(User user);
        Task SignOutAsync();
        int GetKundId();
        int GetAdminId();
    }
}
