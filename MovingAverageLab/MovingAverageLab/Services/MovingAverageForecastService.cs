using MovingAverageLab.Models;

namespace MovingAverageLab.Services
{
    // Прогнозирование методом экстраполяции по скользящей средней
    // Алгоритм: следующее значение = среднее арифметическое последних n значений
    // Каждый новый прогноз добавляется в список и участвует в следующем расчёте

    public class MovingAverageForecastService : IForecastService
    {
        public List<ForecastResult> Forecast(List<HousingRecord> data, int windowSize, int periodsAhead)
        {
            if (data.Count < windowSize)
                throw new ArgumentException(
                    $"Недостаточно данных: нужно минимум {windowSize} записей для окна n={windowSize}.");

            if (periodsAhead <= 0)
                throw new ArgumentException("Количество периодов прогноза должно быть больше нуля.");

            var results = new List<ForecastResult>();

            // Рабочие списки — расширяются по мере добавления прогнозных значений
            var ones   = data.Select(r => r.OneRoom).ToList();
            var twos   = data.Select(r => r.TwoRoom).ToList();
            var threes = data.Select(r => r.ThreeRoom).ToList();

            int lastYear = data.Last().Year;

            for (int i = 0; i < periodsAhead; i++)
            {
                // Берём среднее по последним n значениям
                double forecastOne   = ones.TakeLast(windowSize).Average();
                double forecastTwo   = twos.TakeLast(windowSize).Average();
                double forecastThree = threes.TakeLast(windowSize).Average();

                // Добавляем в рабочий список — он используется на следующей итерации
                ones.Add(forecastOne);
                twos.Add(forecastTwo);
                threes.Add(forecastThree);

                results.Add(new ForecastResult
                {
                    Year      = lastYear + i + 1,
                    OneRoom   = Math.Round(forecastOne,   2),
                    TwoRoom   = Math.Round(forecastTwo,   2),
                    ThreeRoom = Math.Round(forecastThree, 2)
                });
            }

            return results;
        }
    }
}
