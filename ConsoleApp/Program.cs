using ConsoleApp;
using IOC.Dependencies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Contexts;


// Configuración
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();


// Variables de uso
string basePath = @"C:\Users\yenze\Programming\CollegeClass\Electiva 1\Archivo CSV Análisis de Ventas-20250923";
var connectionString = configuration.GetConnectionString("DefaultConnection");


var services = new ServiceCollection();
services.AddETLDependency(connectionString);

var serviceProvider = services.BuildServiceProvider();

using var scope = serviceProvider.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<SADVContext>();

var optionsBuilder = new DbContextOptionsBuilder<SADVContext>();
optionsBuilder.UseNpgsql(connectionString);

var ETL = new ETLRunner(basePath, context);

await ETL.Run();





