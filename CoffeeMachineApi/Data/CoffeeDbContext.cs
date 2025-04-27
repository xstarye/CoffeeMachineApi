using CoffeeMachineApi.Entity;
using Microsoft.EntityFrameworkCore;

namespace CoffeeMachineApi.Data
{
    public class CoffeeDbContext : DbContext
    {
        public CoffeeDbContext(DbContextOptions<CoffeeDbContext> options) : base(options) { }
        public DbSet<DateRule> DateRule { get; set; }

    }
}
