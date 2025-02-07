using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;

namespace FribergCarRentals.Data
{
    public abstract class GenericRepository<T> : IRepository<T> where T : class
    {
        protected readonly AppDbContext context;

        public GenericRepository(AppDbContext context)
        {
            this.context = context;
        }

        public virtual async Task AddAsync(T entity)
        {
            await context.AddAsync(entity);
        }

        public virtual void Remove(T entity)
        {
            context.Remove(entity);
        }

        public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            var entity = await context.Set<T>().FirstOrDefaultAsync(predicate);
            return entity;
        }

        public virtual async Task<IEnumerable<T>?> GetAllAsync()
        {
            var entities = await context.Set<T>().ToListAsync<T>();

            if (entities.Count > 0)
            {
                return entities;
            }

            return null;
        }

        public virtual async Task<T> GetByIdAsync(int id)
        {
            var entity = await context.FindAsync<T>(id);

            return entity ?? throw new Exception($"Objektet kunde inte hittas i databasen - {typeof(T).Name} ID: {id} ");
        }

        public virtual async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }

        public virtual void Update(T entity)
        {
           context.Update<T>(entity);
        }
    }
}
