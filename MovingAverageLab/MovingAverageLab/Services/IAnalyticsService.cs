using MovingAverageLab.Models;

namespace MovingAverageLab.Services
{
    // Результат аналитики — какой тип квартир подорожал больше/меньше
    // record — неизменяемый тип (инкапсуляция данных)
    public record AnalyticsResult(
        string MostGrownType,    // тип квартир с максимальным ростом
        double MostGrownPercent, // процент роста
        string LeastGrownType,   // тип квартир с минимальным ростом
        double LeastGrownPercent
    );

    // Интерфейс аналитики данных о ценах
    public interface IAnalyticsService
    {
        AnalyticsResult Analyze(List<HousingRecord> data);
    }
}
