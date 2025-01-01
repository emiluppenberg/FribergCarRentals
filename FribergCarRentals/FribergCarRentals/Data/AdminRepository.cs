using FribergCarRentals.Models;

namespace FribergCarRentals.Data
{
    public class AdminRepository : GenericRepository<Admin>
    {
        public AdminRepository(AppDbContext context) : base(context)
        {
        }
    }
}
