using CoffeeMachineApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CoffeeMachineApi.Data
{
    public class InventoryDbContext : DbContext
    {
        public DbSet<Stock> stocks { get; set; }
        public InventoryDbContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Stock>().Property(p => p.RowVersion).IsRowVersion();
        }
    }
}
