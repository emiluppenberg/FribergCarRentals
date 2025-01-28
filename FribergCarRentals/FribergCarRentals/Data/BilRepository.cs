using FribergCarRentals.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
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
                .Where(x => x.Bokningar!.Any(x => x.Startdatum <= DateTime.Today && x.Genomförd != true))
                .Include(x => x.Bokningar!
                    .Where(x => x.Startdatum <= DateTime.Today && x.Genomförd != true))
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

        public List<Bil>? GetRandomBilar()
        {
            if (context.Bilar.Count() == 0)
            {
                return null;
            }

            var random = new Random();

            var indices = new List<int>();

            var bilar = new List<Bil>();

            while (bilar.Count < 3)
            {
                var index = random.Next(0, context.Bilar.Count());

                if (!indices.Contains(index))
                {
                    if (context.Bilar.ElementAt(index).ÄrAktiv == true)
                    {
                        bilar.Add(context.Bilar
                        .ElementAt(index));
                    }
                }

                indices.Add(index);

                if (bilar.Count == context.Bilar.Count())
                {
                    break;
                }
            }

            return bilar;
        }
    }
}
