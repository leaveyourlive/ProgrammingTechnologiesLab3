using System;
using System.Windows.Forms;
using System.Drawing;

namespace MovingAverageLab
{
    public class MainMenuForm : Form
    {
        public MainMenuForm()
        {
            SetupMainMenu();
        }

        private void SetupMainMenu()
        {
            // --- Настройки окна ---
            this.Text = "Статистический анализ v1.0";
            this.Size = new Size(450, 650);
            this.BackColor = Color.FromArgb(28, 28, 30);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;

            // --- Заголовок ---
            Label lblTitle = new Label
            {
                Text = "ВЫБЕРИТЕ ЗАДАНИЕ",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 18f),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 100 // Увеличили место под заголовок
            };
            this.Controls.Add(lblTitle);

            // --- Умный контейнер для автоматического выравнивания ---
            FlowLayoutPanel menuPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown, // Кнопки строго друг под другом
                WrapContents = false,
                Padding = new Padding(45,120, 45, 0), // Центрируем по горизонтали
                AutoScroll = false
            };
            this.Controls.Add(menuPanel);

            // --- Создание кнопок (Top теперь не нужен, FlowLayoutPanel сам их расставит) ---

            // Вариант 1
            RoundedButton btn1 = CreateStyledButton("ВАРИАНТ 1: ПРОБЕЖКИ");
            btn1.Click += (s, e) =>
            {
                var form1 = new RunningApp.Forms.MainForm();
                form1.ShowDialog();
            };

            // Вариант 3
            RoundedButton btn3 = CreateStyledButton("ВАРИАНТ 3: ТЕМПЕРАТУРА");
            btn3.Click += (s, e) => new MovingAverageLab.Forms.WeatherForm().Show();

            // Вариант 8
            RoundedButton btn8 = CreateStyledButton("ВАРИАНТ 8: ЗАРПЛАТЫ");
            btn8.Click += (s, e) => MessageBox.Show("Задание 8: Медианная зарплата в РФ.", "Информация");

            // Вариант 9
            RoundedButton btn9 = CreateStyledButton("ВАРИАНТ 9: ЖИЛЬЁ");
            btn8.Click += (s, e) => MessageBox.Show("Задание 9: Анализ цен на первичное жильё в РФ за 15 лет с прогнозом.", "Информация");

            // Разделитель (небольшой отступ перед выходом)
            Panel spacer = new Panel { Height = 30, Width = 10 };

            // Кнопка Выход
            RoundedButton btnExit = CreateStyledButton("ВЫХОД");
            btnExit.BaseColor = Color.FromArgb(235, 52, 52);
            btnExit.HoverColor = Color.FromArgb(200, 30, 30);
            btnExit.Click += (s, e) => Application.Exit();

            // Добавляем в панель (порядок добавления = порядок на экране)
            menuPanel.Controls.Add(btn1);
            menuPanel.Controls.Add(btn3);
            menuPanel.Controls.Add(btn8);
            menuPanel.Controls.Add(btn9);
            menuPanel.Controls.Add(spacer);
            menuPanel.Controls.Add(btnExit);
        }

        private RoundedButton CreateStyledButton(string text)
        {
            return new RoundedButton
            {
                Text = text,
                Width = 340, // Подобрано под размер окна и Padding
                Height = 60,
                Radius = 15,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10f),
                BaseColor = Color.FromArgb(58, 58, 60),
                HoverColor = Color.FromArgb(72, 72, 74),
                Margin = new Padding(0, 10, 0, 10) // Отступы МЕЖДУ кнопками
            };
        }
    }
}