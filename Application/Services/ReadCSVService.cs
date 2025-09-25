using CsvHelper;

namespace Application.Services
{
    public class ReadCSVService<T>
    {

        private readonly string _pathFile;

        public ReadCSVService(string pathfile)
        {
            _pathFile = pathfile;
        }


        public List<T> ReadRecords()
        {

            try
            {
                if (!File.Exists(_pathFile))
                {
                    Console.WriteLine($"File not found: {_pathFile}");
                    return new List<T>();
                }


                using var reader = new StreamReader(_pathFile);
                using var csv = new CsvReader(reader, System.Globalization.CultureInfo.InvariantCulture);

                var records = csv.GetRecords<T>().ToList();

                return records;


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }



            return new List<T>();
        }
    }
}


