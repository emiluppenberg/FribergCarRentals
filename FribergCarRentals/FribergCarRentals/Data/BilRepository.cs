using FribergCarRentals.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

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

        public override async Task<IEnumerable<Bil>?> FindAllAsync(Expression<Func<Bil, bool>> predicate)
        {
            return await context.Bilar
                .Where(predicate)
                .Include(x => x.Bokningar!)
                .ThenInclude(x => x.Kund)
                .ToListAsync();
        }
    }
}
