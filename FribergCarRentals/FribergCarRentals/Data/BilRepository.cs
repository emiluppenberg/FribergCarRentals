using FribergCarRentals.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FribergCarRentals.Data
{
    public class BilRepository : GenericRepository<Bil>, IBilRepository
    {
        public BilRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Bil>?> GetAllWithKommandeBokningarAsync()
        {
            return await context.Bilar
                .Where(x => x.Bokningar!.Any(x => x.Startdatum > DateTime.Today))
                .Include(x => x.Bokningar!
                    .Where(x => x.Startdatum > DateTime.Today))
                .ThenInclude(x => x.Kund)
                .ToListAsync();
        }

        public async Task<IEnumerable<Bil>?> GetAllWithPågåendeBokningarAsync()
        {
            return await context.Bilar
                .Where(x => x.Bokningar!.Any(x => x.Startdatum < DateTime.Today && x.Genomförd != true))
                .Include(x => x.Bokningar!
                    .Where(x => x.Startdatum < DateTime.Today && x.Genomförd != true))
                .ThenInclude(x => x.Kund)
                .ToListAsync();
        }

        public async Task<IEnumerable<Bil>?> GetAllWithAvslutadeBokningarAsync()
        {
            return await context.Bilar
                .Where(x => x.Bokningar!.Any(x => x.Genomförd == true))
                .Include(x => x.Bokningar!
                    .Where(x => x.Genomförd == true))
                .ThenInclude(x => x.Kund)
                .ToListAsync();
        }

        public async Task<IEnumerable<Bil>?> GetAllWithBokningarAsync()
        {
            return await context.Bilar
                .Where(x => x.ÄrAktiv == true)
                .Include(x => x.Bokningar!
                    .Where(x => x.Genomförd != true))
                .ToListAsync();
        }

        public override async Task<IEnumerable<Bil>?> GetAllAsync()
        {
            return await context.Bilar
                .Where(x => x.ÄrAktiv == true)
                .ToListAsync();
        }
    }
}
