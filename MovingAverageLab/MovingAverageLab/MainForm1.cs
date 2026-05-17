using System.Data;
using HousingPriceAnalyzer.Models;
using HousingPriceAnalyzer.Services;

namespace HousingPriceAnalyzer
{
    public class MainForm1 : Form
    {
        private readonly IDataLoader       _dataLoader;
        private readonly IAnalyticsService _analyticsService; // добавлен сервис аналитики

        private List<HousingRecord> _records = new();

        private DataGridView         _dataGrid    = null!;
        private ToolStripButton      _btnCharts   = null!;
        private Label                _lblResult   = null!; // панель аналитики
        private ToolStripStatusLabel _statusLabel = null!;

        public MainForm1()
        {
            _dataLoader       = new JsonDataLoader();
            _analyticsService = new AnalyticsService(); // инициализация
            BuildUI();
        }

        private void BuildUI()
        {
            Text          = "Анализ цен на первичном рынке жилья";
            Size          = new Size(1000, 700);
            MinimumSize   = new Size(900, 600);
            StartPosition = FormStartPosition.CenterScreen;
            Font          = new Font("Segoe UI", 9.5f);

            // Статус-бар
            var statusStrip = new StatusStrip { Dock = DockStyle.Bottom };
            _statusLabel = new ToolStripStatusLabel("Загрузите файл данных...");
            statusStrip.Items.Add(_statusLabel);

            // ToolStrip
            var toolStrip = new ToolStrip
            {
                Dock      = DockStyle.Top,
                GripStyle = ToolStripGripStyle.Hidden,
                Padding   = new Padding(4, 2, 4, 2)
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

            toolStrip.Items.AddRange(new ToolStripItem[]
            {
                btnOpen,
                new ToolStripSeparator(),
                _btnCharts
            });

            // Панель аналитики внизу
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

            // Таблица данных
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
                ShowAnalytics(); // вызываем аналитику после загрузки
                _btnCharts.Enabled = true;
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

        // новый метод — отображает результаты аналитики
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
