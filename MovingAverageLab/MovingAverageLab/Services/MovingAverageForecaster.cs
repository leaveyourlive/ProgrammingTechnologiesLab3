using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovingAverageLab.Services
{
    public class MovingAverageForecaster
    {
        private readonly int _n;

        public MovingAverageForecaster(int n)
        {
            if (n <= 0) throw new ArgumentException("N должен быть больше 0");
            _n = n;
        }

        public List<double> Forecast(List<double> data, int stepsAhead)
        {
            if (data.Count < _n)
                throw new InvalidOperationException($"Нужно минимум {_n} значений");

            var result = new List<double>();
            var working = new List<double>(data);

            for (int i = 0; i < stepsAhead; i++)
            {
                double avg = working.Skip(working.Count - _n).Take(_n).Average();
                avg = Math.Round(avg, 2);
                result.Add(avg);
                working.Add(avg);
            }

            return result;
        }
    }
}
