using FribergCarRentals.Models;
using Microsoft.EntityFrameworkCore;

namespace FribergCarRentals.Data
{
    public class KundRepository : GenericRepository<Kund>
    {
        public KundRepository(AppDbContext context) : base(context)
        {
        }

        public override async Task<Kund> GetByIdAsync(int id)
        {
            return await context.Set<Kund>()
                .Include(x => x.Bokningar)
                .ThenInclude(x => x.Bil)
                .FirstAsync(x => x.Id == id);
        }
    }
}
