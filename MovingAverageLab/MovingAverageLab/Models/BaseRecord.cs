using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Абстрактный базовый класс для записи данных (принцип абстракции ООП)
namespace MovingAverageLab.Models
{
    public abstract class BaseRecord
    {
        // Сериализация записи в словарь
        public abstract Dictionary<string, object> ToDictionary();
        // Проверка корректности данных
        public abstract bool Validate();
    }
}
