using FribergCarRentals.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FribergCarRentals.Data
{
    public class BokningRepository : GenericRepository<Bokning>, IBokningRepository
    {
        public BokningRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Bokning>?> GetAllKundBokningarAsync(int kundId)
        {
            return await context.Bokningar
                .Where(x => x.KundId == kundId)
                .Include(x => x.Bil)
                .ToListAsync();
        }

        public async Task<bool> AnyBetweenDatesAsync(Bokning bokning)
        {
            return await context.Bokningar
                .AnyAsync(
                x => x.BilId == bokning.BilId &&
                x.Startdatum <= bokning.Slutdatum &&
                x.Slutdatum >= bokning.Startdatum &&
                x.Genomförd != true &&
                x.Id != bokning.Id);
        }

        public async Task<Bokning> GetByIdIncludeAsync(int bokningId)
        {
            return await context.Bokningar
                .Include(x => x.Bil)
                    .ThenInclude(x => x.Bokningar)
                .Include(x => x.Kund)
                .FirstAsync(x => x.Id == bokningId);
        }
    }
}
