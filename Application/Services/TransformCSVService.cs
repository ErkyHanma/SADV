using Application.Dtos.CSV;
using Domain.Entities;

namespace Application.Services
{
    public class TransformCSVService
    {
        public List<Client> CleanClient(string pathfile)
        {

            var readCSVService = new BaseReadCSVService<CustomerDto>(pathfile);

            // Clean process

            List<CustomerDto> customers = readCSVService.ReadRecords();

            var clientList = customers.Select(r => new Client
            {
                Id = r.CustomerID,
                Name = $"{r.FirstName} {r.LastName}",
                Email = r.Email,
                City = r.City,
                Country = r.Country,
            })
            .Where(c => !string.IsNullOrEmpty(c.Email) && !string.IsNullOrEmpty(c.City) && !string.IsNullOrEmpty(c.Country))
            .GroupBy(c => c.Email.ToLower())
            .Select(g => g.First())
            .ToList();

            Console.WriteLine(clientList.Count);




            return clientList;

        }
    }
}
