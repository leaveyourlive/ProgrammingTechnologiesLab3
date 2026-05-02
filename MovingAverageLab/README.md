# Лабораторная работа — Скользящая средняя

Статистическое прогнозирование методом экстраполяции по скользящей средней.

## Структура проекта

```
MovingAverageLab/
├── MovingAverageLab.sln          ← решение Visual Studio
└── MovingAverageLab/
    ├── MovingAverageLab.csproj   ← .NET 8 WinForms проект
    ├── Program.cs                ← точка входа
    ├── MainMenuForm.cs           ← главное меню (3 кнопки)
    ├── RoundedButton.cs          ← кастомный элемент управления
    ├── HelpForm.cs               ← справка о методе
    └── README.md
```

## Требования

- .NET 8 SDK (Windows)
- Visual Studio 2022 или Rider

## Быстрый старт

```bash
git clone <repo-url>
cd MovingAverageLab
dotnet run --project MovingAverageLab
```

---

## Для каждого участника: добавление своего варианта

### 1. Создайте новую форму

Добавьте файл `Variant**N**Form.cs` в папку `MovingAverageLab/` (где **N** — номер вашего варианта).  
Минимальный шаблон:

```csharp
namespace MovingAverageLab;

public class VariantNForm : Form
{
    public VariantNForm()
    {
        Text = "Вариант N — <название задания>";
        // ... ваша реализация
    }
}
```

### 2. Подключите форму к кнопке «Открыть задание»

В файле `MainMenuForm.cs` найдите метод `BtnOpen_Click` и замените заглушку:

```csharp
private void BtnOpen_Click(object? sender, EventArgs e)
{
    // TODO: замените заглушку
    var form = new VariantNForm();   // ← ваша форма
    form.Show();
}
```

### 3. Сделайте коммит в свою ветку

```bash
git checkout -b variant-N
git add .
git commit -m "feat: вариант N — <краткое описание>"
git push origin variant-N
```

---

## Варианты заданий

| № | Тема |
|---|------|
| 1 | Пробежки за месяц (км, скорость, пульс) |
| 2 | Курс рубля к двум валютам за месяц |
| 3 | Температура в городе за месяц |
| 4 | ВВП и ВНП России за 15 лет |
| 5 | Численность населения России за 15 лет |
| 6 | Миграция населения России за 15 лет |
| 7 | Браки и разводы в России за 15 лет |
| 8 | Медианная зарплата в России за 15 лет |

Источник данных: [rosstat.gov.ru](https://rosstat.gov.ru/statistic)

---

## Общие требования к реализации варианта

- Загрузка данных из файла (JSON / XML / XLSX — на выбор)
- Вывод данных в таблицу (`DataGridView`)
- Построение графиков зависимости от периода (можно использовать `Microsoft.Data.Analysis` + `LiveCharts2` или `OxyPlot`)
- Специфический расчёт (см. задание своего варианта)
- Прогнозирование методом скользящей средней на **N** периодов вперёд (N вводит пользователь)
- Прогнозные значения выделить другим цветом на графике
