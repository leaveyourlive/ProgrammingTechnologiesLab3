using System.Text.Json;
using MovingAverageLab.Models;

namespace MovingAverageLab.Services
{
    // Загружает данные из JSON-файла
    // Использует встроенный System.Text.Json (.NET 8), без внешних зависимостей
    public class JsonDataLoader : IDataLoader
    {
        // Настройки десериализации — нечувствительность к регистру полей
        private static readonly JsonSerializerOptions _options = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public List<HousingRecord> LoadData(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Файл не найден: {filePath}");

            string json = File.ReadAllText(filePath);

            var records = JsonSerializer.Deserialize<List<HousingRecord>>(json, _options);

            if (records == null || records.Count == 0)
                throw new InvalidDataException("Файл пуст или имеет неверный формат.");

            // Сортируем на случай, если порядок в файле нарушен

            if (records == null || records.Count == 0)
                throw new InvalidDataException("Файл пуст или имеет неверный формат.");

            if (records.Any(r => r.OneRoom <= 0 || r.TwoRoom <= 0 || r.ThreeRoom <= 0))
                throw new InvalidDataException("Файл содержит некорректные данные: цены должны быть больше нуля.");

            // Сортируем на случай, если порядок в файле нарушен
            return records.OrderBy(r => r.Year).ToList();

        }
    }
}
