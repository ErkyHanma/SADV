using Application.Dtos.CSV;
using CsvHelper;
using Domain.Entities;
using Persistence.Contexts;

public class LoadCSVService
{
    private readonly string _pathFile;
    private readonly SADVContext _context;

    public LoadCSVService(string pathFile, SADVContext context)
    {
        _pathFile = pathFile;
        _context = context;
    }

    public async Task ReadCSVAsync()
    {
        if (!File.Exists(_pathFile))
        {
            Console.WriteLine($"Archivo no encontrado: {_pathFile}");
            return;
        }

        using var reader = new StreamReader(_pathFile);
        using var csv = new CsvReader(reader, System.Globalization.CultureInfo.InvariantCulture);

        var records = csv.GetRecords<CustomerDto>().ToList();

        var clientList = records.Select(r => new Client
        {
            Id = r.CustomerID,
            Name = $"{r.FirstName} {r.LastName}",
            Email = r.Email,
            City = r.City,
            Country = r.Country,
        })
        .Where(c => !string.IsNullOrEmpty(c.Email)) // Solo clientes con email
        .GroupBy(c => c.Email.ToLower())           // Evitar duplicados por email
        .Select(g => g.First())
        .Take(10)                                 // Solo los primeros 10
        .ToList();

        try
        {
            // Agregar a la DB
            _context.Clients.AddRange(clientList);
            await _context.SaveChangesAsync();

            Console.WriteLine($"Clientes insertados: {clientList.Count}");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error insertando en la DB: " + ex.Message);
        }


    }
}
