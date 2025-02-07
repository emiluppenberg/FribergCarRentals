using FribergCarRentals.Models;

namespace FribergCarRentals.Data
{
    public interface IBilRepository : IRepository<Bil>
    {
        Task<IEnumerable<Bil>?> GetAllWithKommandeBokningarAsync();
        Task<IEnumerable<Bil>?> GetAllWithPågåendeBokningarAsync();
        Task<IEnumerable<Bil>?> GetAllWithAvslutadeBokningarAsync();
        Task<IEnumerable<Bil>?> GetAllWithBokningarAsync();
        IAsyncEnumerable<Bil>? GetRandomBilarAsync();
    }
}
