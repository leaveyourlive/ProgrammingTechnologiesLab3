using MovingAverageLab.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

// Конкретная реализация загрузчика для JSON-файла
namespace MovingAverageLab.Data
{
    public class JsonSalaryLoader : IDataLoader
    {
        public SalaryDataset Load(string path)
        {
            try
            {
                string jsonContent = File.ReadAllText(path, Encoding.UTF8);
                var jsonDoc = JsonDocument.Parse(jsonContent);
                var root = jsonDoc.RootElement;

                string description = root.TryGetProperty("description", out var desc) ? desc.GetString() : "";

                var records = new List<SalaryRecord>();

                if (root.TryGetProperty("records", out var recordsElement))
                {
                    foreach (var item in recordsElement.EnumerateArray())
                    {
                        int year = item.GetProperty("year").GetInt32();
                        double male = item.GetProperty("male").GetDouble();
                        double female = item.GetProperty("female").GetDouble();

                        var rec = new SalaryRecord(year, male, female);
                        if (rec.Validate())
                        {
                            records.Add(rec);
                        }
                    }
                }

                return new SalaryDataset(records, description);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка загрузки JSON: {ex.Message}", ex);
            }
        }
    }
}
