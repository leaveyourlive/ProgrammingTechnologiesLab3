using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Коллекция записей. Инкапсулирует список и предоставляет методы доступа
namespace MovingAverageLab.Models
{
    public class SalaryDataset
    {
        private List<SalaryRecord> _records;
        private string _description;
        public SalaryDataset(List<SalaryRecord> records, string description = "")
        {
            _records = records;
            _description = description;
        }

        public List<SalaryRecord> Records => _records;

        public List<int> Years => _records.Select(r => r.Year).ToList();

        public List<double> MaleSalaries => _records.Select(r => r.Male).ToList();

        public List<double> FemaleSalaries => _records.Select(r => r.Female).ToList();

        public int Count => _records.Count;

        public string Description => _description;
    }
}
