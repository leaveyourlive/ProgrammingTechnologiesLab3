using System;
using System.Collections.Generic;
using System.Linq;

namespace RunningApp.Forecast
{
    public static class MovingAverage
    {
        public static double PredictNext(List<double> values, int windowSize)
        {
            if (values == null || values.Count == 0) return 0;
            if (windowSize <= 0) windowSize = 1;
            if (values.Count < windowSize) return Math.Round(values.Average(), 2);

            var lastValues = values.Skip(values.Count - windowSize).Take(windowSize);
            return Math.Round(lastValues.Average(), 2);
        }

        public static List<double> ForecastNextDays(List<double> historicalData, int windowSize, int days)
        {
            var forecast = new List<double>();
            var workingData = new List<double>(historicalData);

            for (int i = 0; i < days; i++)
            {
                double nextValue = PredictNext(workingData, windowSize);
                forecast.Add(nextValue);
                workingData.Add(nextValue);
            }
            return forecast;
        }
    }
}