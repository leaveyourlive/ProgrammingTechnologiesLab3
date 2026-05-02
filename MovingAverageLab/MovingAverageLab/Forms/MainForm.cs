using System;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using RunningApp.Models;
using RunningApp.Data;
using RunningApp.Charts;

namespace RunningApp.Forms
{
    public partial class MainForm : Form
    {
        private List<RunningData>? _data;
        private DataGridView dgvData = null!;
        private Chart chartMain = null!;
        private Button btnLoadFile = null!;
        private Button btnShowStats = null!;
        private Label lblWeekendKm = null!;
        private ComboBox cbChartType = null!;

        public MainForm()
        {
            SetupUI();
        }

        private void SetupUI()
        {
            this.Text = "Анализ пробежек";
            this.Size = new Size(1000, 700);

            dgvData = new DataGridView { Location = new Point(20, 20), Size = new Size(700, 200), ReadOnly = true };

            chartMain = new Chart { Location = new Point(20, 240), Size = new Size(700, 400) };
            chartMain.ChartAreas.Add(new ChartArea());

            btnLoadFile = new Button { Text = "Загрузить CSV", Location = new Point(750, 20), Size = new Size(150, 30) };
            btnLoadFile.Click += BtnLoadFile_Click!;

            btnShowStats = new Button { Text = "Статистика", Location = new Point(750, 60), Size = new Size(150, 30), Enabled = false };
            btnShowStats.Click += BtnShowStats_Click!;

            lblWeekendKm = new Label { Text = "Км за выходные: —", Location = new Point(750, 110), Size = new Size(200, 30) };

            cbChartType = new ComboBox { Location = new Point(750, 150), Size = new Size(150, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            cbChartType.Items.AddRange(new object[] { "Дистанция", "Скорость" });
            cbChartType.SelectedIndex = 0;
            cbChartType.SelectedIndexChanged += (s, e) => UpdateChart();

            Controls.Add(dgvData);
            Controls.Add(chartMain);
            Controls.Add(btnLoadFile);
            Controls.Add(btnShowStats);
            Controls.Add(lblWeekendKm);
            Controls.Add(cbChartType);
        }

        private void BtnLoadFile_Click(object? sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "CSV files (*.csv)|*.csv";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    _data = DataLoader.LoadFromCsv(ofd.FileName);
                    dgvData.DataSource = _data.Select(d => new { d.Date, d.DistanceKm, d.DurationMinutes, d.AvgSpeedKmph }).ToList();
                    btnShowStats.Enabled = true;
                    UpdateChart();
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
            lblWeekendKm.Text = $"Км за выходные: {weekendKm:F2} км";
        }
    }
}