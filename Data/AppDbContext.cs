using Microsoft.EntityFrameworkCore;
using CirkulaApi.Models;

namespace CirkulaApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Store> Stores { get; set; }
    }
}
