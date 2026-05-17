using MovingAverageLab.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Абстрактный загрузчик - принцип OCP и полиморфизм
namespace MovingAverageLab.Data
{
    public interface IDataLoader
    {
        SalaryDataset Load(string path);
    }
}
