using System;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using RunningApp.Models;
using RunningApp.Data;
using RunningApp.Forecast;
using RunningApp.Charts;

namespace RunningApp.Forms
{
    public partial class MainForm : Form
    {
        private List<RunningData>? _data;
        private DataGridView dgvData = null!;
        private Chart chartMain = null!;
        private NumericUpDown nudForecastDays = null!;
        private NumericUpDown nudWindowSize = null!;
        private Button btnLoadFile = null!;
        private Button btnShowStats = null!;
        private Button btnForecast = null!;
        private Label lblWeekendKm = null!;
        private ComboBox cbChartType = null!;

        public MainForm()
        {
            SetupUI();
        }

        private void SetupUI()
        {
            this.Text = "Анализ пробежек v2.0";
            this.Size = new Size(1000, 800);
            this.StartPosition = FormStartPosition.CenterScreen;

            dgvData = new DataGridView { Location = new Point(20, 20), Size = new Size(700, 200), ReadOnly = true };

            chartMain = new Chart { Location = new Point(20, 240), Size = new Size(700, 500) };
            chartMain.ChartAreas.Add(new ChartArea());

            btnLoadFile = new Button { Text = " Загрузить CSV", Location = new Point(750, 20), Size = new Size(150, 30) };
            btnLoadFile.Click += BtnLoadFile_Click!;

            btnShowStats = new Button { Text = " Статистика", Location = new Point(750, 60), Size = new Size(150, 30), Enabled = false };
            btnShowStats.Click += BtnShowStats_Click!;

            lblWeekendKm = new Label { Text = " Км за выходные: —", Location = new Point(750, 105), Size = new Size(250, 30) };

            cbChartType = new ComboBox { Location = new Point(750, 250), Size = new Size(150, 40), DropDownStyle = ComboBoxStyle.DropDownList };
            cbChartType.Items.AddRange(new object[] { "Дистанция", "Скорость" });
            cbChartType.SelectedIndex = 0;
            cbChartType.SelectedIndexChanged += (s, e) => UpdateChart();

            Label lblWindow = new Label { Text = "Окно сглаживания:", Location = new Point(750, 180), Size = new Size(120, 25) };
            nudWindowSize = new NumericUpDown { Location = new Point(880, 178), Minimum = 2, Maximum = 10, Value = 3, Size = new Size(60, 25) };

            Label lblDays = new Label { Text = "Дней прогноза:", Location = new Point(750, 215), Size = new Size(120, 25) };
            nudForecastDays = new NumericUpDown { Location = new Point(880, 213), Minimum = 1, Maximum = 30, Value = 5, Size = new Size(60, 25) };

            btnForecast = new Button { Text = " Прогноз", Location = new Point(750, 500), Size = new Size(150, 30), Enabled = false, BackColor = Color.LightCoral };
            btnForecast.Click += BtnForecast_Click!;

            Button btnExport = new Button
            {
                Text = " Экспорт графика",
                Location = new Point(750, 390),
                Size = new Size(150, 30),
                BackColor = Color.LightYellow,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };

            btnExport.Click += BtnExportChart_Click!;
            Controls.Add(btnExport);
            Controls.Add(dgvData);
            Controls.Add(chartMain);
            Controls.Add(btnLoadFile);
            Controls.Add(btnShowStats);
            Controls.Add(lblWeekendKm);
            Controls.Add(cbChartType);
            Controls.Add(lblWindow);
            Controls.Add(nudWindowSize);
            Controls.Add(lblDays);
            Controls.Add(nudForecastDays);
            Controls.Add(btnForecast);
        }

        private void BtnLoadFile_Click(object? sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "CSV files (*.csv)|*.csv";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        _data = DataLoader.LoadFromCsv(ofd.FileName);
                        dgvData.DataSource = _data.Select(d => new { d.Date, d.DistanceKm, d.DurationMinutes, d.AvgSpeedKmph, d.MaxSpeedKmph, d.AvgPulse }).ToList();
                        btnShowStats.Enabled = true;
                        btnForecast.Enabled = true;
                        UpdateChart();
                        MessageBox.Show($"Загружено {_data.Count} дней", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void UpdateChart()
        {
            if (_data == null) return;
            if (cbChartType.SelectedItem?.ToString() == "Дистанция")
                ChartRenderer.DrawDistanceChart(chartMain, _data);
            else
                ChartRenderer.DrawAvgSpeedChart(chartMain, _data);
        }

        private void BtnShowStats_Click(object? sender, EventArgs e)
        {
            if (_data == null) return;
            double weekendKm = _data.Where(d => d.IsWeekend).Sum(d => d.DistanceKm);
            double totalKm = _data.Sum(d => d.DistanceKm);
            lblWeekendKm.Text = $" Км за выходные: {weekendKm:F2} (всего: {totalKm:F2})";

            MessageBox.Show($" Статистика:\nВсего км: {totalKm:F2}\nСредняя скорость: {_data.Average(d => d.AvgSpeedKmph):F2}\nДней с пробежками: {_data.Count(d => d.DistanceKm > 0)}",
                "Детальная статистика", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnForecast_Click(object? sender, EventArgs e)
        {
            if (_data == null) return;

            int days = (int)nudForecastDays.Value;
            int windowSize = (int)nudWindowSize.Value;

            var distances = _data.Select(d => d.DistanceKm).ToList();
            var forecast = MovingAverage.ForecastNextDays(distances, windowSize, days);

            string msg = $" ПРОГНОЗ (окно={windowSize}, дней={days}):\n\n";
            for (int i = 0; i < forecast.Count; i++)
                msg += $"День {i + 1}: {forecast[i]:F2} км\n";

            MessageBox.Show(msg, "Результат прогноза", MessageBoxButtons.OK, MessageBoxIcon.Information);
            ChartRenderer.DrawDistanceChart(chartMain, _data, forecast);
        }
        // Экспорт графика

        private void BtnExportChart_Click(object? sender, EventArgs e)
        {
            if (chartMain == null || chartMain.Series.Count == 0)
            {
                MessageBox.Show("Нет данных для экспорта. Сначала загрузите файл.",
                    "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "PNG Image|*.png|JPEG Image|*.jpg|BMP Image|*.bmp";
                sfd.Title = "Сохранить график";
                sfd.FileName = $"график_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                sfd.DefaultExt = "png";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // Сохраняем график в выбранный формат
                        ChartImageFormat format = ChartImageFormat.Png;
                        if (sfd.FilterIndex == 2) format = ChartImageFormat.Jpeg;
                        else if (sfd.FilterIndex == 3) format = ChartImageFormat.Bmp;

                        chartMain.SaveImage(sfd.FileName, format);

                        MessageBox.Show($" График успешно сохранен!\n\n📁 Путь: {sfd.FileName}",
                            "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($" Ошибка сохранения:\n{ex.Message}",
                            "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}