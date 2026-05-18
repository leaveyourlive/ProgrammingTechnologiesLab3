using MovingAverageLab.Analytics;
using MovingAverageLab.Controls;
using MovingAverageLab.Data;
using MovingAverageLab.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MovingAverageLab.Forms
{
    public partial class SalaryTabForm : Form
    {
        private SalaryDataset _dataset;
        private SalaryAnalytics _analytics;
        private DataGridView _dataGridView;
        private TextBox _statsTextBox;
        private Button _loadButton;
        private Label _statusLabel;
        private Panel _chartPanel;
        private ChartControl _chartControl;

        // Вкладка "Зарплаты"
        public SalaryTabForm()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Text = "Медианная заработная плата в России";
            this.Size = new Size(1400, 1100);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(1100, 700);

            // Главный TableLayoutPanel
            var mainTable = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Padding = new Padding(5)
            };
            mainTable.RowStyles.Clear();
            mainTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 45));  // Кнопки
            mainTable.RowStyles.Add(new RowStyle(SizeType.Percent, 40));   // Таблица
            mainTable.RowStyles.Add(new RowStyle(SizeType.Percent, 60));   // График и статистика

            // Панель кнопок
            var topPanel = new Panel { Height = 45, Dock = DockStyle.Fill };
            _loadButton = new Button { Text = "Загрузить JSON...", Location = new Point(10, 8), Size = new Size(130, 28) };
            _loadButton.Click += LoadFile;
            _statusLabel = new Label { Text = "Файл не загружен", ForeColor = Color.Gray, Location = new Point(150, 12), AutoSize = true };
            topPanel.Controls.AddRange(new Control[] { _loadButton, _statusLabel });

            // Таблица
            _dataGridView = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            // Нижняя часть с графиком и статистикой
            var bottomSplit = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical,
                //SplitterDistance = 400  // Ширина панели статистики
            };

            _statsTextBox = new TextBox
            {
                Dock = DockStyle.Fill,
                Height = 200,
                Width = 500,
                Multiline = true,
                ReadOnly = true,
                Font = new Font("Consolas", 10),
                BackColor = Color.White,
                ScrollBars = ScrollBars.Vertical
            };
            bottomSplit.Panel1.Controls.Add(_statsTextBox);

            _chartPanel = new Panel { Dock = DockStyle.Fill, Height = 400, Width =  900};
            bottomSplit.Panel2.Controls.Add(_chartPanel);

            // Добавляем все в таблицу
            mainTable.Controls.Add(topPanel, 0, 0);
            mainTable.Controls.Add(_dataGridView, 0, 1);
            mainTable.Controls.Add(bottomSplit, 0, 2);

            Controls.Add(mainTable);
        }

        private void LoadFile(object sender, EventArgs e)
        {
            using (var openDialog = new OpenFileDialog())
            {
                openDialog.Filter = "JSON файлы|*.json|Все файлы|*.*";
                openDialog.Title = "Выберите JSON файл с данными о зарплатах";

                if (openDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var loader = new JsonSalaryLoader();
                        _dataset = loader.Load(openDialog.FileName);
                        _analytics = new SalaryAnalytics(_dataset);

                        _statusLabel.Text = $"Загружено: {Path.GetFileName(openDialog.FileName)}";
                        _statusLabel.ForeColor = Color.Green;

                        FillTable();
                        FillStats();
                        BuildChart();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void FillTable()
        {
            _dataGridView.Columns.Clear();
            _dataGridView.Columns.Add("Year", "Год");
            _dataGridView.Columns.Add("Male", "Мужчины, руб.");
            _dataGridView.Columns.Add("Female", "Женщины, руб.");
            _dataGridView.Columns.Add("Difference", "Разница, руб.");

            foreach (var record in _dataset.Records)
            {
                _dataGridView.Rows.Add(
                    record.Year,
                    record.Male.ToString("N0"),
                    record.Female.ToString("N0"),
                    record.Difference.ToString("N0")
                );
            }
        }

        private void FillStats()
        {
            var lines = new List<string>();

            foreach (var gender in new[] { ("male", "Мужчины"), ("female", "Женщины") })
            {
                var maxGrowth = _analytics.MaxGrowth(gender.Item1);
                var minGrowth = _analytics.MinGrowth(gender.Item1);

                lines.Add($"=== {gender.Item2} ===");
                lines.Add($"Макс. рост: {maxGrowth.Change:+0.0;-0.0}% ({maxGrowth.Year})");
                if (minGrowth.Change < 0)
                {
                    lines.Add($"Макс. падение: {minGrowth.Change:+0.0;-0.0}% ({minGrowth.Year})");
                }
                else
                {
                    lines.Add($"Мин. рост: {minGrowth.Change:+0.0;-0.0}% ({minGrowth.Year})");
                }
                lines.Add("");
            }

            _statsTextBox.Text = string.Join(Environment.NewLine, lines);
        }

        private void BuildChart()
        {
            _chartPanel.Controls.Clear();
            _chartControl = new ChartControl(_dataset, _analytics);
            _chartControl.Dock = DockStyle.Fill;
            _chartPanel.Controls.Add(_chartControl);
        }
    }
}
