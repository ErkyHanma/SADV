using Application.Dtos.CSV;
using Application.Services.CSV;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;

namespace ConsoleApp
{
    public class CSVRunner
    {
        private string _basepath;
        private SADVContext _context;

        public CSVRunner(string basepath, SADVContext context)
        {
            _basepath = basepath;
            _context = context;

        }

        public async Task ProcessCSV()
        {
            try
            {

                Console.WriteLine("Starting CSV files processing...\n");

                // ---------
                // Read 
                // ---------
                Console.WriteLine("Reading CSV records...\n");

                var readCSVServiceCustomer = new ReadCSVService<CustomerDto>(Path.Combine(_basepath, "customers.csv"));
                var customers = readCSVServiceCustomer.ReadRecords();

                var readCSVServiceProducts = new ReadCSVService<ProductsDto>(Path.Combine(_basepath, "products.csv"));
                var products = readCSVServiceProducts.ReadRecords();

                var readCSVServiceOrder = new ReadCSVService<OrdersDto>(Path.Combine(_basepath, "orders.csv"));
                var orders = readCSVServiceOrder.ReadRecords();

                var readCSVServiceOrderDetail = new ReadCSVService<OrderDetailDto>(Path.Combine(_basepath, "order_details.csv"));
                var orderDetails = readCSVServiceOrderDetail.ReadRecords();

                var dataSource = new DataSource { SourceType = "CSV" };


                Console.WriteLine(customers.Count > 0 ? $"Total Customer records: {customers.Count}" : "Not Customers found");
                Console.WriteLine(products.Count > 0 ? $"Total Product records: {products.Count}" : "Not Product found");
                Console.WriteLine(orders.Count > 0 ? $"Total Order records: {orders.Count}" : "Not Order found");
                Console.WriteLine(orderDetails.Count > 0 ? $"Total Order Details records: {orderDetails.Count}" : "Not Order Details found");


                // ----------------------------
                // Data Transformation / Cleaning
                // ----------------------------
                Console.WriteLine("\nTransforming records...\n");

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


                using var transaction = await _context.Database.BeginTransactionAsync();
                var csvDataLoader = new LoadCSVService(_context);

                try
                {
                    // For Development
                    _context.Database.ExecuteSqlRaw("DELETE FROM public.\"SaleDetails\"");
                    _context.Database.ExecuteSqlRaw("DELETE FROM public.\"Sales\"");
                    _context.Database.ExecuteSqlRaw("DELETE FROM public.\"Products\"");
                    _context.Database.ExecuteSqlRaw("DELETE FROM public.\"Clients\"");
                    _context.Database.ExecuteSqlRaw("DELETE FROM public.\"DataSources\"");

                    Console.WriteLine("Existing data cleared.\n");

                    Console.WriteLine("Inserting transformed data into database...\n");

                    // Clients
                    await csvDataLoader.LoadCSVRecords<Client>(clientList);
                    Console.WriteLine(clientList.Count > 0 ? $"Inserted Clients: {clientList.Count}" : "Not customers were inserted");

                    // Products
                    await csvDataLoader.LoadCSVRecords<Product>(productList);
                    Console.WriteLine(productList.Count > 0 ? $"Inserted Products: {productList.Count}" : "Not products were inserted");

                    // Orders
                    await csvDataLoader.LoadCSVRecords<Sale>(saleList);
                    Console.WriteLine(saleList.Count > 0 ? $"Inserted Orders: {saleList.Count}" : "Not orders were inserted");

                    // Order Details
                    await csvDataLoader.LoadCSVRecords<SaleDetails>(saleDetailsList);
                    Console.WriteLine(saleDetailsList.Count > 0 ? $"Inserted Order Details: {saleDetailsList.Count}" : "Not Order Details were inserted");

                    // Data Source
                    await csvDataLoader.LoadCSVRecord<DataSource>(dataSource);
                    Console.WriteLine($"Data inserted was type {dataSource.SourceType} loaded in {dataSource.LoadDate} ");

                    await transaction.CommitAsync();

                    Console.WriteLine("\nData load complete.");

                    Console.WriteLine("\nCSV files processing completed!");

                }
                catch (Exception dbEx)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine($"Database error: {dbEx.Message}");
                    throw;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");

            }

        }

    }

}



