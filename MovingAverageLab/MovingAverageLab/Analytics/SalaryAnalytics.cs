using MovingAverageLab.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovingAverageLab.Analytics
{
    // Все вычисления над датасетом зарплат. Принцип SRP - только вычисления
    public class SalaryAnalytics
    {
        private SalaryDataset _dataset;

        public SalaryAnalytics(SalaryDataset dataset)
        {
            _dataset = dataset;
        }

        private List<double> GetPctChanges(List<double> values)
        {
            var changes = new List<double>();
            for (int i = 1; i < values.Count; i++)
            {
                double prev = values[i - 1];
                double curr = values[i];
                if (prev != 0)
                {
                    changes.Add((curr - prev) / prev * 100);
                }
            }
            return changes;
        }

        public List<(int Year, double Change)> MalePctChanges()
        {
            var years = _dataset.Years;
            var values = _dataset.MaleSalaries;
            var changes = GetPctChanges(values);
            var result = new List<(int, double)>();
            for (int i = 0; i < changes.Count; i++)
            {
                result.Add((years[i + 1], changes[i]));
            }
            return result;
        }

        public List<(int Year, double Change)> FemalePctChanges()
        {
            var years = _dataset.Years;
            var values = _dataset.FemaleSalaries;
            var changes = GetPctChanges(values);
            var result = new List<(int, double)>();
            for (int i = 0; i < changes.Count; i++)
            {
                result.Add((years[i + 1], changes[i]));
            }
            return result;
        }

        public (int Year, double Change) MaxGrowth(string gender)
        {
            var data = gender == "male" ? MalePctChanges() : FemalePctChanges();
            return data.OrderByDescending(x => x.Change).First();
        }

        public (int Year, double Change) MinGrowth(string gender)
        {
            var data = gender == "male" ? MalePctChanges() : FemalePctChanges();
            return data.OrderBy(x => x.Change).First();
        }

        public List<double> MovingAverageForecast(List<double> values, int n, int steps)
        {
            var data = new List<double>(values);
            var forecast = new List<double>();

            for (int step = 0; step < steps; step++)
            {
                var window = data.Skip(data.Count - n).Take(n).ToList();
                double nextVal = window.Average();
                forecast.Add(nextVal);
                data.Add(nextVal);
            }

            return forecast;
        }
    }
}
