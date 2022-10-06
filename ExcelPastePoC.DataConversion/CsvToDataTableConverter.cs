using CsvHelper;
using CsvHelper.Configuration;
using System.Data;
using System.Globalization;
using System.IO;

namespace ExcelPastePoC.DataConversion
{
    public class CsvToDataTableConverter
    {
        private readonly CsvConfiguration _conf;

        public CsvToDataTableConverter()
        {
            _conf = new CsvConfiguration(CultureInfo.CurrentCulture);
        }

        public DataTable GetDataTableFromCsv(string csv)
        {
            var value = new DataTable();
            if (string.IsNullOrEmpty(csv))
            {
                return value;
            }
            using (var textReader = new StringReader(csv))
            {
                using (var csvReader = new CsvReader(textReader, _conf))
                {
                    using (var csvDataReader = new CsvDataReader(csvReader))
                    {
                        value.Load(csvDataReader);
                    }
                }
            }
            return value;
        }

    }
}
