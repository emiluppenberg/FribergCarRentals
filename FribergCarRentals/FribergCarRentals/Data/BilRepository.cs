using FribergCarRentals.Models;

namespace FribergCarRentals.Data
{
    public class BilRepository : GenericRepository<Bil>
    {
        public BilRepository(AppDbContext context) : base(context)
        {
        }

        public override void Add(Bil entity)
        {
            base.Add(entity);
        }

        public override Task AddAsync(Bil entity)
        {
            return base.AddAsync(entity);
        }
    }
}
