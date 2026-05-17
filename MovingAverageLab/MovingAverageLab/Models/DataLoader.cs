using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using RunningApp.Models;

namespace RunningApp.Data
{
    public static class DataLoader
    {
        public static List<RunningData> LoadFromCsv(string filePath)
        {
            var result = new List<RunningData>();

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Файл не найден: {filePath}");

            var lines = File.ReadAllLines(filePath);

            for (int i = 1; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i])) continue;

                var parts = lines[i].Split(',');
                if (parts.Length < 6) continue;

                try
                {
                    // Поддержка разных форматов даты
                    string dateStr = parts[0].Trim();
                    if (!DateTime.TryParseExact(dateStr, new[] { "yyyy-MM-dd", "dd.MM.yyyy", "dd/MM/yyyy" },
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
                    {
                        throw new Exception($"Ошибка в строке {i + 1}: неверный формат даты '{dateStr}'");
                    }

                    var data = new RunningData
                    {
                        Date = date,
                        DistanceKm = double.Parse(parts[1].Trim(), CultureInfo.InvariantCulture),
                        DurationMinutes = int.Parse(parts[2].Trim()),
                        AvgSpeedKmph = double.Parse(parts[3].Trim(), CultureInfo.InvariantCulture),
                        MaxSpeedKmph = double.Parse(parts[4].Trim(), CultureInfo.InvariantCulture),
                        AvgPulse = int.Parse(parts[5].Trim())
                    };
                    result.Add(data);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Ошибка в строке {i + 1}: {ex.Message}");
                }
            }

            if (result.Count == 0)
                throw new InvalidDataException("Файл не содержит корректных данных");

            return result;
        }
    }
}