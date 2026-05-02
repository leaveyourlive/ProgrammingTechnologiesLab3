using System;
using System.Collections.Generic;
using System.IO;
using RunningApp.Models;

namespace RunningApp.Data
{
    public static class DataLoader
    {
        public static List<RunningData> LoadFromCsv(string filePath)
        {
            var result = new List<RunningData>();
            var lines = File.ReadAllLines(filePath);

            for (int i = 1; i < lines.Length; i++)
            {
                var parts = lines[i].Split(',');
                var data = new RunningData
                {
                    Date = DateTime.Parse(parts[0]),
                    DistanceKm = double.Parse(parts[1]),
                    DurationMinutes = int.Parse(parts[2]),
                    AvgSpeedKmph = double.Parse(parts[3]),
                    MaxSpeedKmph = double.Parse(parts[4]),
                    AvgPulse = int.Parse(parts[5])
                };
                result.Add(data);
            }
            return result;
        }
    }
}