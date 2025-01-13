using FribergCarRentals.Models;

namespace FribergCarRentals.Data
{
    public class BilRepository : GenericRepository<Bil>
    {
        public BilRepository(AppDbContext context) : base(context)
        {
        }
    }
}
