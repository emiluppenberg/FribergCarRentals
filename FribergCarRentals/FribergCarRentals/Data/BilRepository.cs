using FribergCarRentals.Models;
using Microsoft.EntityFrameworkCore;

namespace FribergCarRentals.Data
{
    public class BilRepository : GenericRepository<Bil>
    {
        public BilRepository(AppDbContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<Bil>?> GetAllAsync()
        {
            return await context.Bilar
                .Include(x => x.Bokningar)
                .ToListAsync();
        }
    }
}
