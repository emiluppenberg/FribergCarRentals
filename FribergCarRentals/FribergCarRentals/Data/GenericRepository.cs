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
        public virtual void Add(T entity)
        {
            context.Add(entity);
        }

        public virtual async Task AddAsync(T entity)
        {
            await context.AddAsync(entity);
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
            return context.Set<T>().ToList();
        }

        public virtual async Task<IEnumerable<T>?> GetAllAsync()
        {
            return await context.Set<T>().ToListAsync<T>();
        }

        public virtual T? GetById(int id)
        {
            return context.Find<T>(id);
        }

        public virtual async Task<T?> GetByIdAsync(int id)
        {
            return await context.FindAsync<T>(id);
        }

        public void SaveChanges()
        {
            context.SaveChanges();
        }

        public async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }

        public void Update(T entity)
        {
           context.Update<T>(entity);
        }
    }
}
