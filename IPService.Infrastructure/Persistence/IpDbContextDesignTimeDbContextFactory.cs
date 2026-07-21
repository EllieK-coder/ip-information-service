using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace IPService.Infrastructure.Persistence
{
    public class IpDbContextDesignTimeDbContextFactory : IDesignTimeDbContextFactory<IpDbContext>
    {
        public IpDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<IpDbContext>();
            optionsBuilder.UseSqlServer("Server=localhost;Database=IpFinderDb;Integrated Security=true;TrustServerCertificate=true;");

            return new IpDbContext(optionsBuilder.Options);
        }
    }
}