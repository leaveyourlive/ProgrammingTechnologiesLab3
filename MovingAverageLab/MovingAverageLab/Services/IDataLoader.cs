using MovingAverageLab.Models;

namespace MovingAverageLab.Services
{
    // Интерфейс загрузки данных из файла
    // Dependency Inversion: зависим от абстракции, не от конкретной реализации
    public interface IDataLoader
    {
        // Загружает список записей из файла, сортирует по году
        List<HousingRecord> LoadData(string filePath);
    }
}
