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

            // Убеждаемся, что ChartArea один
            if (chart.ChartAreas.Count == 0)
                chart.ChartAreas.Add(new ChartArea());

            // Общий фон графика
            chart.BackColor = Color.White;

            // Стиль области графика
            chart.ChartAreas[0].BackColor = Color.FromArgb(240, 248, 255);  // AliceBlue
            chart.ChartAreas[0].BorderColor = Color.SteelBlue;
            chart.ChartAreas[0].BorderWidth = 2;
            chart.ChartAreas[0].ShadowColor = Color.LightGray;
            chart.ChartAreas[0].ShadowOffset = 2;

            // Сетка
            chart.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.LightGray;
            chart.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
            chart.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.LightGray;
            chart.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dash;

            // Шрифты осей
            chart.ChartAreas[0].AxisX.LabelStyle.Font = new Font("Segoe UI", 8);
            chart.ChartAreas[0].AxisY.LabelStyle.Font = new Font("Segoe UI", 8);
            chart.ChartAreas[0].AxisX.TitleFont = new Font("Segoe UI", 9, FontStyle.Bold);
            chart.ChartAreas[0].AxisY.TitleFont = new Font("Segoe UI", 9, FontStyle.Bold);

            // Цвета осей
            chart.ChartAreas[0].AxisX.LineColor = Color.DarkGray;
            chart.ChartAreas[0].AxisY.LineColor = Color.DarkGray;

            // Настройка осей

            chart.ChartAreas[0].AxisX.Title = "День месяца";
            chart.ChartAreas[0].AxisY.Title = "Дистанция (км)";
            chart.ChartAreas[0].AxisX.Minimum = 1;
            chart.ChartAreas[0].AxisX.Maximum = data.Count + (forecast?.Count ?? 0);
            chart.ChartAreas[0].AxisY.Minimum = 0;
            // Автоматический расчёт максимума для дистанции
            double maxDistance = data.Max(d => d.DistanceKm);
            if (forecast != null && forecast.Count > 0)
                maxDistance = Math.Max(maxDistance, forecast.Max());
            chart.ChartAreas[0].AxisY.Maximum = maxDistance + 1; // запас 1 км
            chart.ChartAreas[0].AxisY.IsStartedFromZero = true;
            chart.ChartAreas[0].AxisX.Interval = 1;

            // Факт данные

            var actual = new Series("Факт")
            {
                ChartType = SeriesChartType.Line,
                Color = Color.Blue,
                BorderWidth = 2,
                MarkerStyle = MarkerStyle.Circle,
                MarkerSize = 6,
                MarkerColor = Color.DarkBlue,
                ToolTip = "День #{VALX}: {VALY:F2} км"
            };

            for (int i = 0; i < data.Count; i++)
            {
                actual.Points.AddXY(i + 1, data[i].DistanceKm);
            }
            chart.Series.Add(actual);

            // Прогноз

            if (forecast != null && forecast.Count > 0)
            {
                var forecastSeries = new Series("Прогноз")
                {
                    ChartType = SeriesChartType.Line,
                    Color = Color.Red,
                    BorderWidth = 2,
                    BorderDashStyle = ChartDashStyle.Dash,
                    MarkerStyle = MarkerStyle.Diamond,
                    MarkerSize = 7,
                    MarkerColor = Color.DarkRed,
                    ToolTip = "Прогноз дня #{VALX}: {VALY:F2} км"
                };

                for (int i = 0; i < forecast.Count; i++)
                {
                    forecastSeries.Points.AddXY(data.Count + i + 1, forecast[i]);
                }
                chart.Series.Add(forecastSeries);
            }

            // Легенда

            chart.Legends.Clear();
            var legend = new Legend("Легенда")
            {
                Docking = Docking.Top,
                Font = new Font("Segoe UI", 9),
                BackColor = Color.WhiteSmoke,
                BorderColor = Color.SteelBlue,
                BorderWidth = 1,
                ShadowOffset = 1
            };
            chart.Legends.Add(legend);

            // Заголовок

            var title = new Title("График дистанции пробежек")
            {
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Docking = Docking.Top,
                ForeColor = Color.DarkBlue
            };
            chart.Titles.Add(title);

            // Принудительное обновление
            chart.Invalidate();
            chart.Update();
        }

        public static void DrawAvgSpeedChart(Chart chart, List<RunningData> data)
        {
            if (chart == null || data == null || data.Count == 0) return;

            chart.Series.Clear();
            chart.Titles.Clear();

            if (chart.ChartAreas.Count == 0)
                chart.ChartAreas.Add(new ChartArea());

            // Настройка внешнего вида

            chart.BackColor = Color.White;
            chart.ChartAreas[0].BackColor = Color.FromArgb(240, 255, 240);  // Honeydew
            chart.ChartAreas[0].BorderColor = Color.ForestGreen;
            chart.ChartAreas[0].BorderWidth = 2;
            chart.ChartAreas[0].ShadowOffset = 2;

            chart.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.LightGray;
            chart.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.LightGray;

            chart.ChartAreas[0].AxisX.LabelStyle.Font = new Font("Segoe UI", 8);
            chart.ChartAreas[0].AxisY.LabelStyle.Font = new Font("Segoe UI", 8);
            chart.ChartAreas[0].AxisX.TitleFont = new Font("Segoe UI", 9, FontStyle.Bold);
            chart.ChartAreas[0].AxisY.TitleFont = new Font("Segoe UI", 9, FontStyle.Bold);

            // Настройка осей

            chart.ChartAreas[0].AxisX.Title = "День месяца";
            chart.ChartAreas[0].AxisY.Title = "Скорость (км/ч)";
            chart.ChartAreas[0].AxisX.Minimum = 1;
            chart.ChartAreas[0].AxisX.Maximum = data.Count;
            chart.ChartAreas[0].AxisY.Minimum = 0;
            // Автоматический расчёт максимума для скорости
            double maxSpeed = data.Max(d => d.AvgSpeedKmph);
            chart.ChartAreas[0].AxisY.Maximum = maxSpeed + 2; // запас 2 км/ч
            chart.ChartAreas[0].AxisY.IsStartedFromZero = true;
            chart.ChartAreas[0].AxisX.Interval = 1;

            // Данные

            var series = new Series("Скорость")
            {
                ChartType = SeriesChartType.Line,
                Color = Color.Green,
                BorderWidth = 2,
                MarkerStyle = MarkerStyle.Triangle,
                MarkerSize = 6,
                MarkerColor = Color.DarkGreen,
                ToolTip = "День #{VALX}: {VALY:F2} км/ч"
            };

            for (int i = 0; i < data.Count; i++)
            {
                series.Points.AddXY(i + 1, data[i].AvgSpeedKmph);
            }
            chart.Series.Add(series);

            // Легенда

            chart.Legends.Clear();
            var legend = new Legend("Легенда")
            {
                Docking = Docking.Top,
                Font = new Font("Segoe UI", 9),
                BackColor = Color.WhiteSmoke,
                BorderColor = Color.ForestGreen,
                BorderWidth = 1
            };
            chart.Legends.Add(legend);

            // Заголовок

            var title = new Title("График средней скорости")
            {
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Docking = Docking.Top,
                ForeColor = Color.DarkGreen
            };
            chart.Titles.Add(title);

            chart.Invalidate();
            chart.Update();
        }
    }
}