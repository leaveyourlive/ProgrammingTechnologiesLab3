namespace MovingAverageLab.Models
{
    // Прогнозное значение цены на жильё для одного года
    // Рассчитывается методом экстраполяции по скользящей средней
    public class ForecastResult
    {
        public int Year { get; set; }       // прогнозируемый год
        public double OneRoom { get; set; }   // прогноз для 1-комнатных
        public double TwoRoom { get; set; }   // прогноз для 2-комнатных
        public double ThreeRoom { get; set; } // прогноз для 3-комнатных
    }
}
