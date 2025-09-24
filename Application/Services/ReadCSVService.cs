using Application.Dtos.CSV;
using Application.Interfaces.Repositories;
using CsvHelper;
using Domain.Entities;

namespace Application.Services
{
    public class ReadCSVService
    {
        private readonly string _pathFile;
        private readonly IClientRepository _clientRepository;

        public ReadCSVService(string pathFile, IClientRepository clientRepository)
        {
            _pathFile = pathFile;
            _clientRepository = clientRepository;
        }

        public void ReadCSV()
        {

            try
            {
                if (File.Exists(_pathFile))
                {
                    using (var reader = new StreamReader(_pathFile))

                    using (var csv = new CsvReader(reader, System.Globalization.CultureInfo.InvariantCulture))
                    {
                        var records = csv.GetRecords<CustomerDto>().ToList();


                        List<Client> clientList = records.Select(record => new Client
                        {
                            Id = record.CustomerID,
                            Name = record.FirstName + " " + record.LastName,
                            Email = record.Email,
                            City = record.City,
                            Country = record.Country,
                        }).ToList();


                        try
                        {
                            _clientRepository.AddClientsAsync(clientList);

                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
                        }

                        Console.WriteLine($"Clientes insertados: {_clientRepository.GetCount()}");



                        foreach (var record in records)
                        {
                            Console.WriteLine($"CustomerID: {record.CustomerID}, FirstName: {record.FirstName}, Lastname: {record.LastName}, Email: {record.Email}, City: {record.City}, Country: {record.Country}");
                        }
                    }

                }
                else
                {
                    throw new FileNotFoundException("El archivo no existe.", _pathFile);
                }
            }
            catch (Exception)
            {

                throw;
            }


        }
    }
}
