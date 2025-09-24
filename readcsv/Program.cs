using Application.Services;

public class Program
{
    static void Main()
    {
        ReadCSVService _service = new ReadCSVService(@"C:\Users\yenze\OneDrive\Desktop\Archivo CSV Análisis de Ventas-20250923/customers.csv",);

        try
        {
            _service.ReadCSV();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }
}

//    }
//    var connectionString = "Host=localhost;Port=5432;Database=AnalisisVentaDB;Username=postgres;Password=Dr.yenzel7";

//    using var conn = new NpgsqlConnection(connectionString);

//    try
//    {
//        conn.Open();
//        Console.WriteLine("✅ Connection successful!");
//    }
//    catch (Exception ex)
//    {
//        Console.WriteLine("❌ Connection failed: " + ex.Message);