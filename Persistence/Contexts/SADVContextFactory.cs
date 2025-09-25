using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;


namespace Persistence.Contexts
{

    // To run migrations
    public class SADVContextFactory : IDesignTimeDbContextFactory<SADVContext>
    {
        public SADVContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            var optionsBuilder = new DbContextOptionsBuilder<SADVContext>();
            optionsBuilder.UseNpgsql(connectionString);

            return new SADVContext(optionsBuilder.Options);
        }
    }
}
