using System.Data;
using MovingAverageLab.Models;
using MovingAverageLab.Services;

namespace MovingAverageLab
{
    public class MainForm1 : Form
    {
        private readonly IDataLoader       _dataLoader;
        private readonly IAnalyticsService _analyticsService;
        private readonly IForecastService  _forecastService;

        private List<HousingRecord> _records = new();

        private DataGridView         _dataGrid        = null!;
        private NumericUpDown        _numWindow       = null!;
        private NumericUpDown        _numPeriods      = null!;
        private ToolStripButton      _btnCharts       = null!;
        private ToolStripButton      _btnForecastChart = null!;
        private ToolStripButton      _btnForecastTable = null!;
        private Label                _lblResult       = null!;
        private ToolStripStatusLabel _statusLabel     = null!;

        public MainForm1()
        {
            _dataLoader       = new JsonDataLoader();
            _analyticsService = new AnalyticsService();
            _forecastService  = new MovingAverageForecastService();
            BuildUI();
        }

        private void BuildUI()
        {
            Text          = "Анализ цен на первичном рынке жилья";
            Size          = new Size(1000, 700);
            MinimumSize   = new Size(900, 600);
            StartPosition = FormStartPosition.CenterScreen;
            Font          = new Font("Segoe UI", 9.5f);

            // ── Статус-бар ──
            var statusStrip = new StatusStrip { Dock = DockStyle.Bottom };
            _statusLabel = new ToolStripStatusLabel("Загрузите файл данных...");
            statusStrip.Items.Add(_statusLabel);

            // ── ToolStrip — всё на одной линии ──
            _numWindow  = new NumericUpDown { Size = new Size(52, 22), Minimum = 2, Maximum = 10, Value = 3 };
            _numPeriods = new NumericUpDown { Size = new Size(52, 22), Minimum = 1, Maximum = 15, Value = 5 };

            var toolStrip = new ToolStrip
            {
                Dock      = DockStyle.Top,
                GripStyle = ToolStripGripStyle.Hidden,
                Padding   = new Padding(4, 2, 4, 2),
                ImageScalingSize = new Size(16, 16)
            };

            var btnOpen = new ToolStripButton("📂  Открыть файл")
            {
                DisplayStyle = ToolStripItemDisplayStyle.Text,
                Font         = new Font("Segoe UI", 9.5f)
            };
            btnOpen.Click += OnOpenFile;

            _btnCharts = new ToolStripButton("📊  Графики")
            {
                DisplayStyle = ToolStripItemDisplayStyle.Text,
                Font         = new Font("Segoe UI", 9.5f),
                Enabled      = false
            };
            _btnCharts.Click += OnShowCharts;

            _btnForecastChart = new ToolStripButton("🔮  График прогноза")
            {
                DisplayStyle = ToolStripItemDisplayStyle.Text,
                Font         = new Font("Segoe UI", 9.5f),
                Enabled      = false
            };
            _btnForecastChart.Click += OnShowForecastChart;

            _btnForecastTable = new ToolStripButton("📋  Таблица прогноза")
            {
                DisplayStyle = ToolStripItemDisplayStyle.Text,
                Font         = new Font("Segoe UI", 9.5f),
                Enabled      = false
            };
            _btnForecastTable.Click += OnShowForecastTable;

            toolStrip.Items.AddRange(new ToolStripItem[]
            {
                btnOpen,
                new ToolStripSeparator(),
                new ToolStripLabel("Окно (n):"),
                new ToolStripControlHost(_numWindow),
                new ToolStripLabel("  Прогноз (лет):"),
                new ToolStripControlHost(_numPeriods),
                new ToolStripSeparator(),
                _btnCharts,
                _btnForecastChart,
                _btnForecastTable
            });

            // ── Панель аналитики ──
            var analyticsPanel = new Panel
            {
                Dock      = DockStyle.Bottom,
                Height    = 65,
                BackColor = Color.FromArgb(232, 244, 255),
                Padding   = new Padding(15, 6, 15, 6)
            };

            var lblTitle = new Label
            {
                Text      = "Аналитика за период:",
                Font      = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Dock      = DockStyle.Top,
                Height    = 20,
                TextAlign = ContentAlignment.MiddleLeft
            };

            _lblResult = new Label
            {
                Text      = "Загрузите файл данных для отображения аналитики.",
                Font      = new Font("Segoe UI", 9.5f),
                Dock      = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };

            analyticsPanel.Controls.Add(_lblResult);
            analyticsPanel.Controls.Add(lblTitle);

            // ── Таблица ──
            _dataGrid = new DataGridView
            {
                Dock                  = DockStyle.Fill,
                ReadOnly              = true,
                AllowUserToAddRows    = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode   = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode         = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible     = false,
                BackgroundColor       = SystemColors.Window,
                BorderStyle           = BorderStyle.None,
                GridColor             = Color.LightSteelBlue,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.SteelBlue,
                    ForeColor = Color.White,
                    Font      = new Font("Segoe UI", 10f, FontStyle.Bold),
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                },
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                },
                AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.AliceBlue,
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                },
                RowTemplate = { Height = 28 }
            };

            Controls.Add(_dataGrid);
            Controls.Add(analyticsPanel);
            Controls.Add(toolStrip);
            Controls.Add(statusStrip);
        }

        // ── Обработчики событий ──

        private void OnOpenFile(object? sender, EventArgs e)
        {
            using var dlg = new OpenFileDialog
            {
                Filter = "JSON файлы (*.json)|*.json|Все файлы (*.*)|*.*",
                Title  = "Выберите файл данных о ценах на жильё",
                InitialDirectory = Application.StartupPath
            };

            if (dlg.ShowDialog() != DialogResult.OK) return;

            try
            {
                _records = _dataLoader.LoadData(dlg.FileName);
                FillTable();
                ShowAnalytics();

                _btnCharts.Enabled        = true;
                _btnForecastChart.Enabled = true;
                _btnForecastTable.Enabled = true;
                _statusLabel.Text = $"✓  Загружено {_records.Count} записей  |  {dlg.FileName}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке файла:\n{ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnShowCharts(object? sender, EventArgs e)
        {
            if (_records.Count == 0) return;
            new ChartForm(_records, null).Show(this);
        }

        private void OnShowForecastChart(object? sender, EventArgs e)
        {
            if (_records.Count == 0) return;
            try
            {
                var forecast = _forecastService.Forecast(
                    _records, (int)_numWindow.Value, (int)_numPeriods.Value);
                new ChartForm(_records, forecast).Show(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при расчёте прогноза:\n{ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnShowForecastTable(object? sender, EventArgs e)
        {
            if (_records.Count == 0) return;
            try
            {
                var forecast = _forecastService.Forecast(
                    _records, (int)_numWindow.Value, (int)_numPeriods.Value);
                new ForecastTableForm(forecast).Show(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при расчёте прогноза:\n{ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ── Вспомогательные методы ──

        private void FillTable()
        {
            var table = new DataTable();
            table.Columns.Add("Год",                 typeof(int));
            table.Columns.Add("1-комн. (руб/кв.м)", typeof(string));
            table.Columns.Add("2-комн. (руб/кв.м)", typeof(string));
            table.Columns.Add("3-комн. (руб/кв.м)", typeof(string));

            foreach (var r in _records)
                table.Rows.Add(r.Year,
                    r.OneRoom.ToString("N0"),
                    r.TwoRoom.ToString("N0"),
                    r.ThreeRoom.ToString("N0"));

            _dataGrid.DataSource = table;
        }

        private void ShowAnalytics()
        {
            if (_records.Count < 2) { _lblResult.Text = "Недостаточно данных."; return; }

            var result = _analyticsService.Analyze(_records);
            int y1 = _records.First().Year;
            int y2 = _records.Last().Year;

            _lblResult.Text =
                $"Больше всего подорожали: {result.MostGrownType} — +{result.MostGrownPercent:F1}% ({y1}–{y2})" +
                $"     |     " +
                $"Меньше всего подорожали: {result.LeastGrownType} — +{result.LeastGrownPercent:F1}%";
        }
    }
}
