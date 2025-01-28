using FribergCarRentals.Models;

namespace FribergCarRentals.Data
{
    public interface IBokningRepository : IRepository<Bokning>
    {
        Task<IEnumerable<Bokning>?> GetAllKundBokningarAsync(int kundId);
        Task<bool> AnyBetweenDatesAsync(Bokning bokning);
        Task<Bokning> GetByIdIncludeAsync(int bokningId);
    }
}
