using System;
using System.Collections.Generic;
using System.Linq;

namespace RunningApp.Forecast
{

    /// Класс для прогнозирования методом скользящей средней

    public static class MovingAverage
    {

        /// Прогнозирование следующего значения методом скользящей средней

        public static double PredictNext(List<double> values, int windowSize, bool ignoreZeros = true)
        {
            if (values == null || values.Count == 0) return 0;
            if (windowSize <= 0) windowSize = 1;

            var lastValues = values.Skip(values.Count - windowSize).Take(windowSize).ToList();

            // Игнорируем нули для более точного прогноза
            if (ignoreZeros)
            {
                var nonZeroValues = lastValues.Where(v => v > 0).ToList();
                if (nonZeroValues.Count > 0)
                    return Math.Round(nonZeroValues.Average(), 2);
            }

            if (values.Count < windowSize) return Math.Round(values.Average(), 2);

            return Math.Round(lastValues.Average(), 2);
        }

        /// Прогноз на N дней вперед

        public static List<double> ForecastNextDays(List<double> historicalData, int windowSize, int days, bool ignoreZeros = true)
        {
            // Оптимизация: проверка входных параметров
            if (historicalData == null || historicalData.Count == 0)
                return new List<double>();

            if (days <= 0)
                return new List<double>();

            // Оптимизация: используем Capacity для уменьшения переаллокаций
            var forecast = new List<double>(days);
            var workingData = new List<double>(historicalData);

            for (int i = 0; i < days; i++)
            {
                double nextValue = PredictNext(workingData, windowSize, ignoreZeros);
                forecast.Add(nextValue);
                workingData.Add(nextValue);
            }

            return forecast;
        }


        // Расчет доверительного интервала прогноза 
        public static (double Min, double Max) GetConfidenceInterval(List<double> historicalData, int windowSize)
        {
            if (historicalData == null || historicalData.Count == 0)
                return (0, 0);

            double avg = historicalData.Average();
            double stdDev = Math.Sqrt(historicalData.Sum(v => Math.Pow(v - avg, 2)) / historicalData.Count);
            double nextValue = PredictNext(historicalData, windowSize);

            return (Math.Round(nextValue - stdDev, 2), Math.Round(nextValue + stdDev, 2));
        }
    }
}