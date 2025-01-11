using FribergCarRentals.Models;

namespace FribergCarRentals.Data
{
    public class KundRepository : GenericRepository<Kund>
    {
        public KundRepository(AppDbContext context) : base(context)
        {
        }
    }
}
