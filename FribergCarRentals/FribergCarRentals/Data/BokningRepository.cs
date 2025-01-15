using FribergCarRentals.Models;

namespace FribergCarRentals.Data
{
    public class BokningRepository : GenericRepository<Bokning>
    {
        public BokningRepository(AppDbContext context) : base(context)
        {
        }
    }
}
