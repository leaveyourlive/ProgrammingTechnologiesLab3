using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovingAverageLab.Models;
using System.Globalization;

namespace MovingAverageLab.Services
{
    public class WeatherCsvLoader
    {
        public List<WeatherRecord> Load(string filePath)
        {
            var records = new List<WeatherRecord>();
            var lines = File.ReadAllLines(filePath);

            foreach (var line in lines.Skip(1))
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                var parts = line.Split(',');
                if (parts.Length < 5) continue;

                try
                {
                    records.Add(new WeatherRecord
                    {
                        Date = DateTime.Parse(parts[0].Trim()),
                        MaxTemp = double.Parse(parts[1].Trim(), CultureInfo.InvariantCulture),
                        MinTemp = double.Parse(parts[2].Trim(), CultureInfo.InvariantCulture),
                        AvgTemp = double.Parse(parts[3].Trim(), CultureInfo.InvariantCulture),
                        Description = parts[4].Trim()
                    });
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Не удалось разобрать строку: {line}. Ошибка: {ex.Message}");
                    continue;
                }
            }

            return records;
        }
    }
}