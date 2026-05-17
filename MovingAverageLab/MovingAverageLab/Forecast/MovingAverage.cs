using System;
using System.Collections.Generic;
using System.Linq;

namespace RunningApp.Forecast
{

    /// Класс для прогнозирования методом скользящей средней

    public static class MovingAverage
    {

        /// Прогнозирование следующего значения методом скользящей средней

        public static double PredictNext(List<double> values, int windowSize)
        {
            // Проверка на пустые данные
            if (values == null || values.Count == 0)
                return 0;

            // Корректировка размера окна
            if (windowSize <= 0)
                windowSize = 1;

            // Если данных меньше окна - берем среднее всех
            if (values.Count < windowSize)
                return Math.Round(values.Average(), 2);

            // Берем последние windowSize значений
            var lastValues = values.Skip(values.Count - windowSize).Take(windowSize);
            return Math.Round(lastValues.Average(), 2);
        }

        /// Прогноз на N дней вперед

        public static List<double> ForecastNextDays(List<double> historicalData, int windowSize, int days)
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
                double nextValue = PredictNext(workingData, windowSize);
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