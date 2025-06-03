using Microsoft.EntityFrameworkCore;
using SmartNameplate.Api.Entities;

namespace Data
{
    public class SmartNameplateContext : DbContext
    {
        public SmartNameplateContext(DbContextOptions<SmartNameplateContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("Host=localhost;Database=smart_nameplate;Username=postgres;Password=postgres");
            }
        }
    }
} 