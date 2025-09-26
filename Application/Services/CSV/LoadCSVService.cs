using Persistence.Contexts;

namespace Application.Services.CSV
{
    public class LoadCSVService
    {
        private readonly SADVContext _context;
        public LoadCSVService(SADVContext context)
        {
            _context = context;
        }

        public async Task LoadCSVRecords<T>(IEnumerable<T> data) where T : class
        {
            try
            {
                if (data != null)
                {
                    await _context.Set<T>().AddRangeAsync(data);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while inserting data");

            }

        }

        public async Task LoadCSVRecord<T>(T data) where T : class
        {

            try
            {
                if (data != null)
                {
                    await _context.Set<T>().AddAsync(data);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while inserting data");

            }

        }
    }
}
