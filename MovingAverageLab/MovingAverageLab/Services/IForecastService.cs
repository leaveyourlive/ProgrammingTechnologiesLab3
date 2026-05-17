using MovingAverageLab.Models;

namespace MovingAverageLab.Services
{
    // Интерфейс прогнозирования цен
    // Open/Closed: можно легко заменить алгоритм не меняя остальной код
    public interface IForecastService
    {
        // data — исходные данные
        // windowSize — размер окна скользящей средней (n)
        // periodsAhead — на сколько лет вперёд считать прогноз
        List<ForecastResult> Forecast(List<HousingRecord> data, int windowSize, int periodsAhead);
    }
}
