using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Contexts;


namespace IOC.Dependencies
{
    public static class ETLDependencies
    {
        public static IServiceCollection AddETLDependency(this IServiceCollection services, string connectionString)
        {
            // DbContext
            services.AddDbContextFactory<SADVContext>(options =>
            {
                options.UseNpgsql(connectionString);
            });

            return services;
        }
    }
}
