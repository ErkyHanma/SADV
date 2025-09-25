using Application.Dtos.CSV;
using Application.Services;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;

namespace ConsoleApp
{
    public class ETLRunner
    {
        private string _basepath;
        private SADVContext _context;

        public ETLRunner(string basepath, SADVContext context)
        {
            _basepath = basepath;
            _context = context;

        }

        public async Task Run()
        {

            // ---------
            // Read 
            // ---------
            Console.WriteLine("Reading CSV records...\n");

            var readCSVService1 = new ReadCSVService<CustomerDto>(Path.Combine(_basepath, "customers.csv"));
            var customers = readCSVService1.ReadRecords();

            var readCSVService4 = new ReadCSVService<ProductsDto>(Path.Combine(_basepath, "products.csv"));
            var products = readCSVService4.ReadRecords();

            var readCSVService3 = new ReadCSVService<OrdersDto>(Path.Combine(_basepath, "orders.csv"));
            var orders = readCSVService3.ReadRecords();

            var readCSVService2 = new ReadCSVService<OrderDetailDto>(Path.Combine(_basepath, "order_details.csv"));
            var orderDetails = readCSVService2.ReadRecords();

            Console.WriteLine($"Total Customer records: {customers.Count}");
            Console.WriteLine($"Total Product records: {products.Count}");
            Console.WriteLine($"Total Order records: {orders.Count}");
            Console.WriteLine($"Total Order Details records: {orderDetails.Count}\n");

            // ----------------------------
            // Data Transformation / Cleaning
            // ----------------------------
            Console.WriteLine("Transforming records...\n");

            // Customers: Remove duplicates & null/empty values
            var clientList = customers
                .Select(r => new Client
                {
                    Id = r.CustomerID,
                    Name = $"{r.FirstName} {r.LastName}",
                    Email = r.Email,
                    PhoneNumber = r.Phone,
                    City = r.City,
                    Country = r.Country,
                })
                .Where(c => !string.IsNullOrEmpty(c.Email) && !string.IsNullOrEmpty(c.City) && !string.IsNullOrEmpty(c.Country))
                .GroupBy(c => c.Email.ToLower())
                .Select(g => g.First())
                .ToList();

            // Products: Remove duplicates, ensure positive price, normalize name + category
            var productList = products
                .Select(p => new Product
                {
                    ProductId = p.ProductID,
                    Category = p.Category,
                    Name = p.ProductName,
                    Price = p.Price,
                    StockQuantity = p.Stock,
                })
                .Where(p => !string.IsNullOrEmpty(p.Name) && p.Price >= 0)
                .GroupBy(p => new { NormalizedName = p.Name.Trim().ToLower(), p.Category })
                .Select(g => g.First())
                .ToList();

            // Orders: Keep only orders for existing clients
            var clientIdList = clientList.Select(c => c.Id).ToHashSet();
            var saleList = orders
                .Select(s => new Sale
                {
                    SaleId = s.OrderID,
                    ClientId = s.CustomerID,
                    Date = s.OrderDate,
                    Status = s.Status
                })
                .Where(s => s.SaleId > 0 && s.ClientId > 0 && !string.IsNullOrWhiteSpace(s.Status))
                .Where(o => clientIdList.Contains(o.ClientId))
                .GroupBy(s => s.SaleId)
                .Select(g => g.First())
                .ToList();

            // Order Details: Aggregate quantities, calculate totals
            var saleIdList = saleList.Select(o => o.SaleId).ToHashSet();
            var productIdList = productList.Select(p => p.ProductId).ToHashSet();
            var productDict = productList.ToDictionary(p => p.ProductId, p => p.Price);

            var saleDetailsList = orderDetails
                .Where(od => saleIdList.Contains(od.OrderID) && productIdList.Contains(od.ProductID))
                .GroupBy(od => new { od.OrderID, od.ProductID })
                .Select(g =>
                {
                    var detail = g.First();
                    var unitPrice = productDict.TryGetValue(detail.ProductID, out var price) ? price : 0;

                    return new SaleDetails
                    {
                        SaleId = detail.OrderID,
                        ProductId = detail.ProductID,
                        Quantity = detail.Quantity,
                        UnitPrice = unitPrice,
                        Total = detail.Quantity * unitPrice
                    };
                })
                .ToList();


            Console.WriteLine("Data transformation complete.\n");

            // ------------
            // Load Data
            // ------------
            Console.WriteLine("Clearing existing data from tables...\n");

            _context.Database.ExecuteSqlRaw("DELETE FROM public.\"SaleDetails\"");
            _context.Database.ExecuteSqlRaw("DELETE FROM public.\"Sales\"");
            _context.Database.ExecuteSqlRaw("DELETE FROM public.\"Products\"");
            _context.Database.ExecuteSqlRaw("DELETE FROM public.\"Clients\"");

            Console.WriteLine("Existing data cleared.\n");

            Console.WriteLine("Inserting transformed data into database...");

            // Clients
            _context.Clients.AddRange(clientList);
            await _context.SaveChangesAsync();
            Console.WriteLine($"Inserted Clients: {clientList.Count}");

            // Products
            _context.Products.AddRange(productList);
            await _context.SaveChangesAsync();
            Console.WriteLine($"Inserted Products: {productList.Count}");

            // Orders
            _context.Sales.AddRange(saleList);
            await _context.SaveChangesAsync();
            Console.WriteLine($"Inserted Orders: {saleList.Count}");

            // Order Details
            _context.SaleDetails.AddRange(saleDetailsList);
            await _context.SaveChangesAsync();
            Console.WriteLine($"Inserted Order Details: {saleDetailsList.Count}");

            Console.WriteLine("\nData load complete.");
        }

    }
}



