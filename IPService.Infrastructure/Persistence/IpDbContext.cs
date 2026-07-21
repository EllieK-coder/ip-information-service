using IPService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace IPService.Infrastructure.Persistence
{
    public class IpDbContext : DbContext
    {
        public DbSet<Ipv4Address> IpAddresses => Set<Ipv4Address>();
        public DbSet<Country> Countries => Set<Country>();
        //public DbSet<JobHistory> JobHistories => Set<JobHistory>();

        public IpDbContext(DbContextOptions<IpDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(IpDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}