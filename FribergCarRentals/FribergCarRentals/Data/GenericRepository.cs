using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;

namespace FribergCarRentals.Data
{
    public abstract class GenericRepository<T> : IRepository<T> where T : class
    {
        private readonly AppDbContext context;

        public GenericRepository(AppDbContext context)
        {
            this.context = context;
        }
        public virtual T? Add(T entity)
        {
            if (entity != null)
            {
                return context.Add(entity).Entity;
            }

            return null;
        }

        public virtual async Task<T?> AddAsync(T entity)
        {
            if (entity != null)
            {
                await context.AddAsync(entity);
                return entity;
            }

            return null;
        }

        public virtual void Remove(T entity)
        {
            context.Remove(entity);
        }

        public virtual T? FirstOrDefault(Expression<Func<T, bool>> predicate)
        {
            return context.Set<T>().FirstOrDefault(predicate);
        }

        public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            var entity = await context.Set<T>().FirstOrDefaultAsync(predicate);
            return entity;
        }

        public virtual IEnumerable<T>? GetAll()
        {
            var entities = context.Set<T>().ToList();

            if (entities.Count > 0)
            {
                return entities;
            }

            return null;
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

        public virtual T? GetById(int id)
        {
            return context.Find<T>(id);
        }

        public virtual async Task<T?> GetByIdAsync(int id)
        {
            var entity = await context.FindAsync<T>(id);

            return entity;
        }

        public void SaveChanges()
        {
            context.SaveChanges();
        }

        public async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }

        public virtual T? Update(T entity)
        {
            return context.Update<T>(entity).Entity;
        }
    }
}
