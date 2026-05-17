using MovingAverageLab.Analytics;
using MovingAverageLab.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;

namespace MovingAverageLab.Controls
{
    // Виджет с интерактивным графиком зарплат
    public class ChartControl : UserControl
    {
        private SalaryDataset _dataset;
        private SalaryAnalytics _analytics;
        private Chart _chart;
        private ComboBox _fromYearCombo;
        private ComboBox _toYearCombo;
        private NumericUpDown _nWindow;
        private NumericUpDown _stepsForecast;
        private Button _buildButton;
        private Button _exportPngButton;

        public ChartControl(SalaryDataset dataset, SalaryAnalytics analytics)
        {
            _dataset = dataset;
            _analytics = analytics;
            InitializeComponents();
            PlotChart();
        }

        private void InitializeComponents()
        {
            var panel = new Panel { Dock = DockStyle.Top, Height = 50 };

            var years = _dataset.Years;

            var fromLabel = new Label { Text = "Год от:", Left = 10, Top = 15, Width = 50 };
            _fromYearCombo = new ComboBox
            {
                Left = 60,
                Top = 12,
                Width = 70,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _fromYearCombo.Items.AddRange(years.Select(y => y.ToString()).ToArray());
            _fromYearCombo.SelectedItem = years[0].ToString();

            var toLabel = new Label { Text = "до:", Left = 140, Top = 15, Width = 30 };
            _toYearCombo = new ComboBox
            {
                Left = 170,
                Top = 12,
                Width = 70,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _toYearCombo.Items.AddRange(years.Select(y => y.ToString()).ToArray());
            _toYearCombo.SelectedItem = years[years.Count - 1].ToString();

            var nLabel = new Label { Text = "Окно N:", Left = 250, Top = 15, Width = 60 };
            _nWindow = new NumericUpDown
            {
                Left = 310,
                Top = 10,
                Width = 50,
                Minimum = 2,
                Maximum = 10,
                Value = 3
            };

            var stepsLabel = new Label { Text = "Прогноз (лет):", Left = 370, Top = 15, Width = 80 };
            _stepsForecast = new NumericUpDown
            {
                Left = 450,
                Top = 10,
                Width = 50,
                Minimum = 1,
                Maximum = 20,
                Value = 5
            };

            _buildButton = new Button { Text = "Построить", Left = 510, Top = 10, Width = 80 };
            _buildButton.Click += (s, e) => PlotChart();

            _exportPngButton = new Button { Text = "Экспорт PNG", Left = 600, Top = 10, Width = 90 };
            _exportPngButton.Click += ExportPng;

            panel.Controls.AddRange(new Control[]
            {
                fromLabel, _fromYearCombo, toLabel, _toYearCombo,
                nLabel, _nWindow, stepsLabel, _stepsForecast,
                _buildButton, _exportPngButton
            });

            _chart = new Chart { Dock = DockStyle.Fill };
            var chartArea = new ChartArea();
            _chart.ChartAreas.Add(chartArea);
            _chart.Legends.Add(new Legend());

            Controls.Add(_chart);
            Controls.Add(panel);
        }

        private void PlotChart()
        {
            int yFrom = int.Parse(_fromYearCombo.SelectedItem.ToString());
            int yTo = int.Parse(_toYearCombo.SelectedItem.ToString());
            int n = (int)_nWindow.Value;
            int steps = (int)_stepsForecast.Value;

            // Фильтрация по диапазону
            var filteredRecords = _dataset.Records.Where(r => r.Year >= yFrom && r.Year <= yTo).ToList();

            if (filteredRecords.Count < 2)
            {
                MessageBox.Show("Недостаточно данных для выбранного диапазона.", "Предупреждение",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var years = filteredRecords.Select(r => r.Year).ToList();
            var male = filteredRecords.Select(r => r.Male).ToList();
            var female = filteredRecords.Select(r => r.Female).ToList();

            // Прогноз
            int lastYear = years.Last();
            var forecastYears = Enumerable.Range(lastYear + 1, steps).ToList();
            var maleForecast = _analytics.MovingAverageForecast(male, n, steps);
            var femaleForecast = _analytics.MovingAverageForecast(female, n, steps);

            _chart.Series.Clear();

            // Мужчины факт
            var maleSeries = new Series("Мужчины (факт)")
            {
                ChartType = SeriesChartType.Line,
                Color = Color.FromArgb(31, 78, 121),
                BorderWidth = 2,
                MarkerStyle = MarkerStyle.Circle
            };
            for (int i = 0; i < years.Count; i++)
            {
                maleSeries.Points.AddXY(years[i], male[i]);
            }
            _chart.Series.Add(maleSeries);

            // Женщины факт
            var femaleSeries = new Series("Женщины (факт)")
            {
                ChartType = SeriesChartType.Line,
                Color = Color.FromArgb(192, 57, 43),
                BorderWidth = 2,
                MarkerStyle = MarkerStyle.Square
            };
            for (int i = 0; i < years.Count; i++)
            {
                femaleSeries.Points.AddXY(years[i], female[i]);
            }
            _chart.Series.Add(femaleSeries);

            // Мужчины прогноз
            var maleForecastSeries = new Series($"Мужчины (прогноз, N={n})")
            {
                ChartType = SeriesChartType.Line,
                Color = Color.FromArgb(91, 163, 217),
                BorderWidth = 1,
                BorderDashStyle = ChartDashStyle.Dash,
                MarkerStyle = MarkerStyle.Circle
            };
            for (int i = 0; i < forecastYears.Count; i++)
            {
                maleForecastSeries.Points.AddXY(forecastYears[i], maleForecast[i]);
            }
            _chart.Series.Add(maleForecastSeries);

            // Женщины прогноз
            var femaleForecastSeries = new Series($"Женщины (прогноз, N={n})")
            {
                ChartType = SeriesChartType.Line,
                Color = Color.FromArgb(229, 115, 115),
                BorderWidth = 1,
                BorderDashStyle = ChartDashStyle.Dash,
                MarkerStyle = MarkerStyle.Square
            };
            for (int i = 0; i < forecastYears.Count; i++)
            {
                femaleForecastSeries.Points.AddXY(forecastYears[i], femaleForecast[i]);
            }
            _chart.Series.Add(femaleForecastSeries);

            _chart.Titles.Clear();
            _chart.Titles.Add("Медианная заработная плата в России");
            _chart.ChartAreas[0].AxisX.Title = "Год";
            _chart.ChartAreas[0].AxisY.Title = "Зарплата, руб./мес.";
            _chart.ChartAreas[0].AxisX.Interval = 1;
        }

        private void ExportPng(object sender, EventArgs e)
        {
            using (var saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = "PNG файлы|*.png";
                saveDialog.DefaultExt = "png";
                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    _chart.SaveImage(saveDialog.FileName, ChartImageFormat.Png);
                    MessageBox.Show($"Сохранено: {saveDialog.FileName}", "Экспорт",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}
