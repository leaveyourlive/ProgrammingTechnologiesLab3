using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovingAverageLab.Models
{
    public class WeatherRecord
    {
        public DateTime Date { get; set; }
        public double MaxTemp { get; set; }
        public double MinTemp { get; set; }
        public double AvgTemp { get; set; }
        public string Description { get; set; } = string.Empty;

        public double Spread => MaxTemp - MinTemp;
    }
}