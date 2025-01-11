using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Linq.Expressions;

namespace FribergCarRentals.Data
{
    public interface IRepository<T> where T : class
    {
        T? GetById(int id);
        Task<T?> GetByIdAsync(int id);
        IEnumerable<T>? GetAll();
        Task<IEnumerable<T>?> GetAllAsync();
        T? FirstOrDefault(Expression<Func<T, bool>> predicate);
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
        void Add(T entity);
        Task AddAsync(T entity);
        void Update(T entity);
        void Remove(T entity);
        void SaveChanges();
        Task SaveChangesAsync();
    }
}
