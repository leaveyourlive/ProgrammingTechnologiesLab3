using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Одна запись: год + медианная зарплата мужчин и женщин
namespace MovingAverageLab.Models
{
    public class SalaryRecord : BaseRecord
    {
        private int _year;
        private double _male;
        private double _female;

        public SalaryRecord(int year, double male, double female)
        {
            _year = year;
            _male = male;
            _female = female;
        }

        // Инкапсуляция через свойства
        public int Year => _year;

        public double Male => _male;

        public double Female => _female;

        public double Difference => _male - _female;

        public override Dictionary<string, object> ToDictionary()
        {
            return new Dictionary<string, object>
            {
                { "year", _year },
                { "male", _male },
                { "female", _female }
            };
        }

        public override bool Validate()
        {
            return (_year >= 2000 && _year <= 2100 && _male > 0 && _female > 0);
        }
    }
}
