using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms.DataVisualization.Charting;
using RunningApp.Models;

namespace RunningApp.Charts
{
    public static class ChartRenderer
    {
        public static void DrawDistanceChart(Chart chart, List<RunningData> data)
        {
            chart.Series.Clear();
            var series = new Series("Дистанция")
            {
                ChartType = SeriesChartType.Line,
                Color = Color.Blue,
                BorderWidth = 2
            };

            for (int i = 0; i < data.Count; i++)
                series.Points.AddXY(i + 1, data[i].DistanceKm);

            chart.Series.Add(series);
            chart.ChartAreas[0].AxisX.Title = "День";
            chart.ChartAreas[0].AxisY.Title = "км";
        }

        public static void DrawAvgSpeedChart(Chart chart, List<RunningData> data)
        {
            chart.Series.Clear();
            var series = new Series("Скорость")
            {
                ChartType = SeriesChartType.Line,
                Color = Color.Green,
                BorderWidth = 2
            };

            for (int i = 0; i < data.Count; i++)
                series.Points.AddXY(i + 1, data[i].AvgSpeedKmph);

            chart.Series.Add(series);
            chart.ChartAreas[0].AxisX.Title = "День";
            chart.ChartAreas[0].AxisY.Title = "км/ч";
        }
    }
}