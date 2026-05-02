using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms.DataVisualization.Charting;
using RunningApp.Models;

namespace RunningApp.Charts
{
    public static class ChartRenderer
    {
        public static void DrawDistanceChart(Chart chart, List<RunningData> data, List<double>? forecast = null)
        {
            if (chart == null || data == null || data.Count == 0) return;

            chart.Series.Clear();
            chart.Titles.Clear();

            if (chart.ChartAreas.Count == 0)
                chart.ChartAreas.Add(new ChartArea());
            else
                chart.ChartAreas[0].AxisX.CustomLabels.Clear();

            // Настройка осей
            chart.ChartAreas[0].AxisX.Title = "День месяца";
            chart.ChartAreas[0].AxisY.Title = "Дистанция (км)";
            chart.ChartAreas[0].AxisX.Minimum = 1;
            chart.ChartAreas[0].AxisX.Maximum = data.Count + (forecast?.Count ?? 0);
            chart.ChartAreas[0].AxisY.Minimum = 0;
            chart.ChartAreas[0].AxisY.Maximum = 12;
            chart.ChartAreas[0].AxisY.IsStartedFromZero = true;
            chart.ChartAreas[0].AxisX.Interval = 1;

            // факт данные (синяя линия)
            var actual = new Series("Факт")
            {
                ChartType = SeriesChartType.Line,
                Color = Color.Blue,
                BorderWidth = 2,
                MarkerStyle = MarkerStyle.Circle,
                MarkerSize = 5
            };

            for (int i = 0; i < data.Count; i++)
            {
                actual.Points.AddXY(i + 1, data[i].DistanceKm);
            }
            chart.Series.Add(actual);

            // Прогноз (красный пунктир)
            if (forecast != null && forecast.Count > 0)
            {
                var forecastSeries = new Series("Прогноз")
                {
                    ChartType = SeriesChartType.Line,
                    Color = Color.Red,
                    BorderWidth = 2,
                    BorderDashStyle = ChartDashStyle.Dash,
                    MarkerStyle = MarkerStyle.Diamond,
                    MarkerSize = 6
                };

                for (int i = 0; i < forecast.Count; i++)
                {
                    forecastSeries.Points.AddXY(data.Count + i + 1, forecast[i]);
                }
                chart.Series.Add(forecastSeries);
            }

            chart.Legends.Clear();
            chart.Legends.Add(new Legend("Легенда") { Docking = Docking.Top });

            chart.Invalidate();
            chart.Update();
        }

        public static void DrawAvgSpeedChart(Chart chart, List<RunningData> data)
        {
            if (chart == null || data == null || data.Count == 0) return;

            chart.Series.Clear();
            chart.Titles.Clear();

            // ChartArea один
            if (chart.ChartAreas.Count == 0)
                chart.ChartAreas.Add(new ChartArea());

            chart.ChartAreas[0].AxisX.Title = "День месяца";
            chart.ChartAreas[0].AxisY.Title = "Скорость (км/ч)";
            chart.ChartAreas[0].AxisX.Minimum = 1;
            chart.ChartAreas[0].AxisX.Maximum = data.Count;
            chart.ChartAreas[0].AxisY.Minimum = 0;
            chart.ChartAreas[0].AxisY.Maximum = 16;
            chart.ChartAreas[0].AxisY.IsStartedFromZero = true;
            chart.ChartAreas[0].AxisX.Interval = 1;

            // Данные
            var series = new Series("Скорость")
            {
                ChartType = SeriesChartType.Line,
                Color = Color.Green,
                BorderWidth = 2,
                MarkerStyle = MarkerStyle.Triangle,
                MarkerSize = 5
            };

            for (int i = 0; i < data.Count; i++)
            {
                series.Points.AddXY(i + 1, data[i].AvgSpeedKmph);
            }
            chart.Series.Add(series);

            chart.Legends.Clear();
            chart.Legends.Add(new Legend("Легенда") { Docking = Docking.Top });

            chart.Invalidate();
            chart.Update();
        }
    }
}