using ConsoleApp;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Persistence.Contexts;


var configuration = new ConfigurationBuilder()
          .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
          .Build();

var connectionString = configuration.GetConnectionString("DefaultConnection");



var optionsBuilder = new DbContextOptionsBuilder<SADVContext>();
optionsBuilder.UseNpgsql(connectionString);

string basePath = @"C:\Users\yenze\OneDrive\Desktop\Archivo CSV Análisis de Ventas-20250923/";

using var context = new SADVContext(optionsBuilder.Options);

var ETL = new ETLRunner(basePath, context);

await ETL.Run();





