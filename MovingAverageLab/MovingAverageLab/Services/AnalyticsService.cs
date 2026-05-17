using MovingAverageLab.Models;

namespace MovingAverageLab.Services
{
    // Вычисляет какой тип квартир подорожал больше и меньше всего
    // за весь период наблюдения (от первого года к последнему)
    public class AnalyticsService : IAnalyticsService
    {
        public AnalyticsResult Analyze(List<HousingRecord> data)
        {
            if (data.Count < 2)
                throw new ArgumentException("Для анализа нужно минимум 2 записи.");

            var first = data.First();
            var last  = data.Last();

            // Считаем процент роста для каждого типа квартир
            var growths = new Dictionary<string, double>
            {
                ["1-комнатные"] = CalcGrowth(first.OneRoom,   last.OneRoom),
                ["2-комнатные"] = CalcGrowth(first.TwoRoom,   last.TwoRoom),
                ["3-комнатные"] = CalcGrowth(first.ThreeRoom, last.ThreeRoom)
            };

            var mostGrown  = growths.MaxBy(kv => kv.Value);
            var leastGrown = growths.MinBy(kv => kv.Value);

            return new AnalyticsResult(
                mostGrown.Key,  mostGrown.Value,
                leastGrown.Key, leastGrown.Value
            );
        }

        // Процент изменения от начального значения к конечному
        private static double CalcGrowth(double initial, double final)
        {
            if (initial == 0) return 0;
            return (final - initial) / initial * 100.0;
        }
    }
}
