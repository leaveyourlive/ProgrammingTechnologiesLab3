using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
using OxyPlot.WindowsForms;
using HousingPriceAnalyzer.Models;

namespace MovingAverageLab
{
    // Окно с графиком цен. Поддерживает:
    // — зум (колесо мыши), пан (ЛКМ + перетаскивание)
    // — выбор диапазона лет
    // — экспорт PNG / SVG

    public class ChartForm : Form
    {
        private readonly List<HousingRecord>  _records;
        private readonly List<ForecastResult>? _forecast;

        private PlotView      _plotView  = null!;
        private NumericUpDown _numFrom   = null!;
        private NumericUpDown _numTo     = null!;

        public ChartForm(List<HousingRecord> records, List<ForecastResult>? forecast)
        {
            _records  = records;
            _forecast = forecast;
            BuildUI();
            BuildChart();
        }

        private void BuildUI()
        {
            bool hasForecast = _forecast is { Count: > 0 };

            Text          = hasForecast ? "Прогноз цен — скользящая средняя" : "Графики цен на жильё";
            Size          = new Size(1050, 680);
            MinimumSize   = new Size(700, 450);
            StartPosition = FormStartPosition.CenterParent;
            Font          = new Font("Segoe UI", 9.5f);

            // ── График ──
            _plotView = new PlotView { Dock = DockStyle.Fill };

            // ── Нижняя панель: диапазон + экспорт ──
            var bottomPanel = new Panel
            {
                Dock      = DockStyle.Bottom,
                Height    = 50,
                BackColor = Color.FromArgb(245, 245, 245),
                Padding   = new Padding(8, 8, 8, 8)
            };

            int minYear = _records.First().Year;
            int maxYear = _forecast is { Count: > 0 }
                ? _forecast.Last().Year
                : _records.Last().Year;

            // Выбор диапазона
            var lblRange = new Label
            {
                Text      = "Диапазон лет:",
                Location  = new Point(8, 14),
                AutoSize  = true
            };

            _numFrom = new NumericUpDown
            {
                Location = new Point(110, 11),
                Size     = new Size(65, 26),
                Minimum  = minYear,
                Maximum  = maxYear,
                Value    = minYear
            };

            var lblDash = new Label
            {
                Text     = "—",
                Location = new Point(180, 14),
                AutoSize = true
            };

            _numTo = new NumericUpDown
            {
                Location = new Point(195, 11),
                Size     = new Size(65, 26),
                Minimum  = minYear,
                Maximum  = maxYear,
                Value    = maxYear
            };

            var btnApply = new Button
            {
                Text     = "Применить",
                Location = new Point(270, 10),
                Size     = new Size(95, 28),
                UseVisualStyleBackColor = true
            };
            btnApply.Click += (_, _) => BuildChart();

            // Экспорт
            var btnPng = new Button
            {
                Text     = "Экспорт PNG",
                Location = new Point(400, 10),
                Size     = new Size(110, 28),
                UseVisualStyleBackColor = true
            };
            btnPng.Click += (_, _) => ExportAs(".png");

            var btnSvg = new Button
            {
                Text     = "Экспорт SVG",
                Location = new Point(520, 10),
                Size     = new Size(110, 28),
                UseVisualStyleBackColor = true
            };
            btnSvg.Click += (_, _) => ExportAs(".svg");

            var lblHint = new Label
            {
                Text      = "Зум: колесо мыши  |  Пан: ЛКМ + перетаскивание",
                Location  = new Point(650, 16),
                AutoSize  = true,
                ForeColor = Color.Gray
            };

            bottomPanel.Controls.AddRange(new Control[]
            {
                lblRange, _numFrom, lblDash, _numTo, btnApply,
                btnPng, btnSvg, lblHint
            });

            Controls.Add(_plotView);
            Controls.Add(bottomPanel);
        }

        private void BuildChart()
        {
            // Фильтруем данные по выбранному диапазону
            int fromYear = (int)_numFrom.Value;
            int toYear   = (int)_numTo.Value;

            if (fromYear > toYear)
            {
                MessageBox.Show("Начальный год не может быть больше конечного.",
                    "Ошибка диапазона", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var filtered = _records
                .Where(r => r.Year >= fromYear && r.Year <= toYear)
                .ToList();

            bool hasForecast = _forecast is { Count: > 0 };

            var model = new PlotModel
            {
                Title         = hasForecast
                    ? "Фактические данные и прогноз (скользящая средняя)"
                    : "Средние цены на первичном рынке жилья",
                TitleFontSize = 14,
                Background    = OxyColors.White
            };

            // Оси
            model.Axes.Add(new LinearAxis
            {
                Position       = AxisPosition.Bottom,
                Title          = "Год",
                MajorStep      = 1,
                MinorStep      = 1,
                StringFormat   = "0",
                MinimumPadding = 0.05,
                MaximumPadding = 0.05
            });
            model.Axes.Add(new LinearAxis
            {
                Position       = AxisPosition.Left,
                Title          = "Цена, руб/кв.м",
                StringFormat   = "N0",
                MinimumPadding = 0.05,
                MaximumPadding = 0.05
            });

            // Легенда
            model.Legends.Add(new Legend
            {
                LegendPosition   = LegendPosition.TopLeft,
                LegendBackground = OxyColor.FromArgb(200, 255, 255, 255),
                LegendBorder     = OxyColors.LightGray,
                LegendFontSize   = 10
            });

            // Исторические данные
            AddLine(model,
                filtered.Select(r => (r.Year, r.OneRoom)),
                "1-комн. (факт)", OxyColors.RoyalBlue, LineStyle.Solid);
            AddLine(model,
                filtered.Select(r => (r.Year, r.TwoRoom)),
                "2-комн. (факт)", OxyColors.ForestGreen, LineStyle.Solid);
            AddLine(model,
                filtered.Select(r => (r.Year, r.ThreeRoom)),
                "3-комн. (факт)", OxyColors.Crimson, LineStyle.Solid);

            // Прогнозные данные (пунктир)
            if (hasForecast)
            {
                var forecastFiltered = _forecast!
                    .Where(f => f.Year <= toYear)
                    .ToList();

                if (forecastFiltered.Count > 0)
                {
                    // Начинаем от последней реальной точки в диапазоне
                    var lastReal = filtered.LastOrDefault() ?? _records.Last();

                    AddLine(model,
                        Prepend((lastReal.Year, lastReal.OneRoom),
                            forecastFiltered.Select(f => (f.Year, f.OneRoom))),
                        "1-комн. (прогноз)", OxyColors.CornflowerBlue, LineStyle.Dash);
                    AddLine(model,
                        Prepend((lastReal.Year, lastReal.TwoRoom),
                            forecastFiltered.Select(f => (f.Year, f.TwoRoom))),
                        "2-комн. (прогноз)", OxyColors.MediumSeaGreen, LineStyle.Dash);
                    AddLine(model,
                        Prepend((lastReal.Year, lastReal.ThreeRoom),
                            forecastFiltered.Select(f => (f.Year, f.ThreeRoom))),
                        "3-комн. (прогноз)", OxyColors.LightCoral, LineStyle.Dash);
                }
            }

            _plotView.Model = model;
        }

        private void ExportAs(string ext)
        {
            string filter = ext == ".png"
                ? "PNG изображение (*.png)|*.png"
                : "SVG векторный файл (*.svg)|*.svg";

            using var dlg = new SaveFileDialog { Filter = filter, FileName = "housing_chart" };
            if (dlg.ShowDialog() != DialogResult.OK) return;

            try
            {
                if (ext == ".png")
                {
                    var bmp = new Bitmap(_plotView.Width, _plotView.Height);
                    _plotView.DrawToBitmap(bmp, new Rectangle(0, 0, _plotView.Width, _plotView.Height));
                    bmp.Save(dlg.FileName, System.Drawing.Imaging.ImageFormat.Png);
                }
                else
                {
                    var exporter = new OxyPlot.SvgExporter { Width = _plotView.Width, Height = _plotView.Height };
                    using var stream = File.OpenWrite(dlg.FileName);
                    exporter.Export(_plotView.Model, stream);
                }
                MessageBox.Show("График успешно экспортирован!", "Экспорт",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при экспорте:\n{ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void AddLine(
            PlotModel model,
            IEnumerable<(int Year, double Price)> points,
            string title, OxyColor color, LineStyle style)
        {
            var series = new LineSeries
            {
                Title           = title,
                Color           = color,
                LineStyle       = style,
                StrokeThickness = 2.2,
                MarkerType      = MarkerType.Circle,
                MarkerSize      = 4,
                MarkerFill      = color
            };
            series.Points.AddRange(points.Select(p => new DataPoint(p.Year, p.Price)));
            model.Series.Add(series);
        }

        private static IEnumerable<(int, double)> Prepend(
            (int, double) first, IEnumerable<(int, double)> rest)
        {
            yield return first;
            foreach (var item in rest) yield return item;
        }
    }
}
