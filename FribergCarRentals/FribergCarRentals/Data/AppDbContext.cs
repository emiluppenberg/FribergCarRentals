using FribergCarRentals.Models;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace FribergCarRentals.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Bil> Bilar { get; set; } = null!;
        public DbSet<Kund> Kunder { get; set; } = null!;
        public DbSet<Bokning> Bokningar { get; set; } = null!;
        public DbSet<Admin> Admins { get; set; } = null!;
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}
