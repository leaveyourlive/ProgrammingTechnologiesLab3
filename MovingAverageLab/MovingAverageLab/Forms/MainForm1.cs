using System.Data;
using HousingPriceAnalyzer.Models;
using HousingPriceAnalyzer.Services;

namespace MovingAverageLab.Forms
{
    public class MainForm1 : Form
    {
        private readonly IDataLoader _dataLoader;

        private List<HousingRecord> _records = new();

        private DataGridView         _dataGrid    = null!;
        private ToolStripStatusLabel _statusLabel = null!;

        public MainForm1()
        {
            _dataLoader = new JsonDataLoader();
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

            // ToolStrip — панель инструментов
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

            toolStrip.Items.Add(btnOpen);

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
                _statusLabel.Text = $"✓  Загружено {_records.Count} записей  |  {dlg.FileName}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке файла:\n{ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
    }
}
