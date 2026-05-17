using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.WindowsForms;
using MovingAverageLab.Models;
using MovingAverageLab.Services;

namespace MovingAverageLab.Forms
{
    public class WeatherForm : Form
    {
        private List<WeatherRecord> _data = new();
        private readonly WeatherCsvLoader _loader = new();
        private DataGridView _grid = null!;
        private PlotView _plotView = null!;
        private Label _lblSpread = null!;
        private NumericUpDown _numN = null!;
        private NumericUpDown _numDayFrom = null!;
        private NumericUpDown _numDayTo = null!;
        private Label _lblStats = null!;

        public WeatherForm()
        {
            Text = "Вариант 3 — Температура в городе";
            Size = new Size(1710, 735);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(28, 28, 30);
            ForeColor = Color.White;

            // Верхняя панель с кнопками
            var topPanel = new Panel { Dock = DockStyle.Top, Height = 40, BackColor = Color.FromArgb(44, 44, 46) };

            var btnLoad = new Button
            {
                Text = "Открыть CSV",
                Width = 120,
                Height = 30,
                Left = 8,
                Top = 5,
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                BackColor = Color.FromArgb(58, 58, 60)
            };
            btnLoad.Click += BtnLoad_Click;

            var btnExport = new Button
            {
                Text = "Сохранить график",
                Width = 140,
                Height = 30,
                Left = 136,
                Top = 5,
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                BackColor = Color.FromArgb(58, 58, 60)
            };
            btnExport.Click += BtnExport_Click;

            var btnBack = new Button 
            { 
                Text = "← Назад",
                Width = 80,
                Height = 30,
                Left = 290,
                Top = 7,
                BackColor = Color.FromArgb(235, 52, 52),
                ForeColor = Color.White, FlatStyle = FlatStyle.Flat 
            };
            btnBack.Click += (s, e) => Close();
            topPanel.Controls.Add(btnBack);

            topPanel.Controls.Add(btnLoad);
            topPanel.Controls.Add(btnExport);


            var filterPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 40,
                BackColor = Color.FromArgb(36, 36, 38)
            };

            var lblFrom = new Label { Text = "День с:", ForeColor = Color.LightGray, AutoSize = true, Left = 10, Top = 12 };

            _numDayFrom = new NumericUpDown { Minimum = 1, Maximum = 31, Value = 1, Width = 55, Left = 60, Top = 10, BackColor = Color.FromArgb(58, 58, 60), ForeColor = Color.White };

            var lblTo = new Label { Text = "по:", ForeColor = Color.LightGray, AutoSize = true, Left = 125, Top = 12 };

            _numDayTo = new NumericUpDown { Minimum = 1, Maximum = 31, Value = 31, Width = 55, Left = 150, Top = 10, BackColor = Color.FromArgb(58, 58, 60), ForeColor = Color.White };

            var btnApply = new Button { Text = "Применить", Width = 90, Height = 28, Left = 215, Top = 7, BackColor = Color.FromArgb(52, 120, 200), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnApply.Click += BtnApply_Click;

            filterPanel.Controls.AddRange(new Control[] { lblFrom, _numDayFrom, lblTo, _numDayTo, btnApply });
            Controls.Add(filterPanel);
            Controls.Add(topPanel);

            // Нижняя панель: перепад + прогноз
            var bottomPanel = new Panel { Dock = DockStyle.Bottom, Height = 50, BackColor = Color.FromArgb(44, 44, 46) };

            _lblSpread = new Label
            {
                Text = "Загрузите CSV файл",
                AutoSize = false,
                Width = 480,
                Height = 20,
                Left = 8,
                TextAlign = ContentAlignment.MiddleLeft
            };

            var lblN = new Label
            {
                Text = "N дней:",
                AutoSize = true,
                Left = 500,
                Top = 12
            };

            _numN = new NumericUpDown
            {
                Minimum = 1,
                Maximum = 30,
                Value = 5,
                Width = 55,
                Left = 555,
                Top = 10
            };

            _lblStats = new Label
            {
                ForeColor = Color.LightGray,
                AutoSize = false,
                Width = 480,
                Height = 20,
                Left = 8,
                Top = 22,
                Font = new Font("Segoe UI", 8f)
            };
            bottomPanel.Controls.Add(_lblStats);

            var btnForecast = new Button
            {
                Text = "Прогноз",
                Width = 80,
                Height = 28,
                Left = 618,
                Top = 7
            };
            btnForecast.Click += BtnForecast_Click;

            bottomPanel.Controls.AddRange(new Control[]
                { _lblSpread, lblN, _numN, btnForecast });
            Controls.Add(bottomPanel);

            // Разделитель: слева таблица, справа график
            var split = new SplitContainer
            {
                Dock = DockStyle.None,
                Width = 1700,
                Height = 580,
                Top = 75,
                Orientation = Orientation.Vertical
            };

            _grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.FromArgb(60, 60, 60),
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(60, 60, 60),
                    ForeColor = Color.White
                },
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(80, 80, 80),
                    ForeColor = Color.White
                },
                EnableHeadersVisualStyles = false
            };

            _plotView = new PlotView { Dock = DockStyle.Fill };

            split.Panel1.Controls.Add(_grid);
            split.Panel2.Controls.Add(_plotView);
            Controls.Add(split);
        }

        private void ShowDescriptionStats()
        {
            var stats = _data
                .GroupBy(r => r.Description)
                .OrderByDescending(g => g.Count())
                .Select(g => $"{g.Key}: {g.Count()} дн.")
                .ToList();

            _lblStats.Text = "Погода: " + string.Join("   ", stats);
        }
        private void BtnApply_Click(object? sender, EventArgs e)
        {
            if (_data.Count == 0) return;

            int from = (int)_numDayFrom.Value - 1;
            int to = (int)_numDayTo.Value;
            if (from >= to)
            {
                MessageBox.Show("Неверный диапазон дней.", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var filtered = _data.Skip(from).Take(to - from).ToList();
            BuildChart(filtered);
        }

        private void BtnLoad_Click(object? sender, EventArgs e)
        {
            using var dlg = new OpenFileDialog { Filter = "CSV|*.csv" };
            if (dlg.ShowDialog() != DialogResult.OK) return;

            _data = _loader.Load(dlg.FileName);
            if (_data.Count == 0)
            {
                MessageBox.Show("Файл пустой или неверный формат.", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            FillTable();
            BuildChart(_data);
            ShowSpreadInfo();

            _numDayTo.Maximum = _data.Count;
            _numDayTo.Value = _data.Count;
            _numDayFrom.Maximum = _data.Count;

            ShowDescriptionStats();
        }

        private void FillTable()
        {
            _grid.Columns.Clear();
            _grid.Rows.Clear();

            _grid.Columns.Add("date", "Дата");
            _grid.Columns.Add("max", "Макс (°C)");
            _grid.Columns.Add("min", "Мин (°C)");
            _grid.Columns.Add("avg", "Средняя (°C)");
            _grid.Columns.Add("spread", "Перепад");
            _grid.Columns.Add("desc", "Описание");

            var maxSpread = _data.Max(r => r.Spread);
            var minSpread = _data.Min(r => r.Spread);

            foreach (var r in _data)
            {
                int idx = _grid.Rows.Add(
                    r.Date.ToString("dd.MM"),
                    r.MaxTemp.ToString("F1"),
                    r.MinTemp.ToString("F1"),
                    r.AvgTemp.ToString("F1"),
                    r.Spread.ToString("F1"),
                    r.Description
                );

                if (r.Spread == maxSpread)
                    _grid.Rows[idx].DefaultCellStyle.BackColor = Color.FromArgb(100, 20, 20);
                else if (r.Spread == minSpread)
                    _grid.Rows[idx].DefaultCellStyle.BackColor = Color.FromArgb(20, 20, 100);

                _grid.Rows[idx].Cells["max"].Style.ForeColor = Color.Red;
                _grid.Rows[idx].Cells["min"].Style.ForeColor = Color.Blue;
            }
        }

        private void BuildChart(List<WeatherRecord> records)
        {
            var model = new PlotModel { Title = "Температура по дням", Background = OxyColor.FromRgb(28, 28, 30), TextColor = OxyColors.LightGray};

            model.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Title = "День"
            });
            model.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Температура (°C)"
            });

            model.Series.Add(MakeSeries("Максимум",
                records.Select(r => r.MaxTemp).ToList(), OxyColors.Red));
            model.Series.Add(MakeSeries("Минимум",
                records.Select(r => r.MinTemp).ToList(), OxyColors.Blue));
            model.Series.Add(MakeSeries("Средняя",
                records.Select(r => r.AvgTemp).ToList(), OxyColors.Green));

            _plotView.Model = model;
        }

        private static LineSeries MakeSeries(string title, List<double> values, OxyColor color)
        {
            var s = new LineSeries
            {
                Title = title,
                Color = color,
                StrokeThickness = 2
            };
            for (int i = 0; i < values.Count; i++)
                s.Points.Add(new DataPoint(i + 1, values[i]));
            return s;
        }

        private void ShowSpreadInfo()
        {
            var maxDay = _data.MaxBy(r => r.Spread)!;
            var minDay = _data.MinBy(r => r.Spread)!;
            _lblSpread.Text =
                $"Макс. перепад: {maxDay.Date:dd.MM} ({maxDay.Spread:F1}°C)   " +
                $"Мин. перепад: {minDay.Date:dd.MM} ({minDay.Spread:F1}°C)";
        }

        private void BtnForecast_Click(object? sender, EventArgs e)
        {
            if (_data.Count == 0) return;

            int n = (int)_numN.Value;

            var avgValues = _data.Select(r => r.AvgTemp).ToList();
            var forecaster = new MovingAverageForecaster();

            try
            {
                var forecast = forecaster.Forecast(avgValues, n, n);
                var model = _plotView.Model;

                // Убираем старый прогноз если есть
                var old = model.Series.FirstOrDefault(s => s.Title == "Прогноз");
                if (old != null) model.Series.Remove(old);

                var forecastSeries = new LineSeries
                {
                    Title = "Прогноз",
                    Color = OxyColors.Orange,
                    StrokeThickness = 2,
                    LineStyle = LineStyle.Dash
                };

                // Начинаем с последней реальной точки
                forecastSeries.Points.Add(new DataPoint(_data.Count, avgValues.Last()));
                for (int i = 0; i < forecast.Count; i++)
                    forecastSeries.Points.Add(new DataPoint(_data.Count + i + 1, forecast[i]));

                model.Series.Add(forecastSeries);
                model.InvalidatePlot(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnExport_Click(object? sender, EventArgs e)
        {
            if (_plotView.Model == null) return;

            using var dlg = new SaveFileDialog
            {
                Filter = "PNG|*.png",
                FileName = "weather_chart"
            };
            if (dlg.ShowDialog() != DialogResult.OK) return;

            using var stream = File.Create(dlg.FileName);
            var exporter = new OxyPlot.WindowsForms.PngExporter
            {
                Width = 1200,
                Height = 600
            };
            exporter.Export(_plotView.Model, stream);
            MessageBox.Show("График сохранён.", "Готово",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}