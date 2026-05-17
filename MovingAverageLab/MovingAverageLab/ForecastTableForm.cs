using System.Data;
using MovingAverageLab.Models;

namespace MovingAverageLab
{
    // Отдельное окно для отображения прогнозных значений в виде таблицы
    public class ForecastTableForm : Form
    {
        private readonly List<ForecastResult> _forecast;
        private DataGridView _dataGrid = null!;

        public ForecastTableForm(List<ForecastResult> forecast)
        {
            _forecast = forecast;
            BuildUI();
            FillTable();
        }

        private void BuildUI()
        {
            Text          = "Таблица прогноза цен";
            Size          = new Size(600, 400);
            MinimumSize   = new Size(500, 300);
            StartPosition = FormStartPosition.CenterParent;
            Font          = new Font("Segoe UI", 9.5f);

            // Заголовок окна с пояснением метода
            var lblTitle = new Label
            {
                Text      = "Прогноз рассчитан методом экстраполяции по скользящей средней",
                Dock      = DockStyle.Top,
                Height    = 35,
                TextAlign = ContentAlignment.MiddleCenter,
                Font      = new Font("Segoe UI", 9.5f, FontStyle.Italic),
                ForeColor = Color.SteelBlue,
                BackColor = Color.FromArgb(232, 244, 255)
            };

            // Таблица с прогнозными значениями
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
                    // Прогнозные строки выделяем слегка другим цветом
                    BackColor = Color.FromArgb(255, 250, 230),
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                },
                RowTemplate = { Height = 28 }
            };

            Controls.Add(_dataGrid);
            Controls.Add(lblTitle);
        }

        private void FillTable()
        {
            var table = new DataTable();
            table.Columns.Add("Год (прогноз)",         typeof(int));
            table.Columns.Add("1-комн. (руб/кв.м)",   typeof(string));
            table.Columns.Add("2-комн. (руб/кв.м)",   typeof(string));
            table.Columns.Add("3-комн. (руб/кв.м)",   typeof(string));

            foreach (var f in _forecast)
                table.Rows.Add(
                    f.Year,
                    f.OneRoom.ToString("N0"),
                    f.TwoRoom.ToString("N0"),
                    f.ThreeRoom.ToString("N0"));

            _dataGrid.DataSource = table;
        }
    }
}
