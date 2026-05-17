namespace MovingAverageLab.Models
{
    // Данные о ценах на первичное жильё за один год (по данным Росстат)
    // Цена указывается в рублях за 1 кв.м
    public class HousingRecord
    {
        public int Year { get; set; }       // год наблюдения
        public double OneRoom { get; set; }   // цена 1-комнатных квартир
        public double TwoRoom { get; set; }   // цена 2-комнатных квартир
        public double ThreeRoom { get; set; } // цена 3-комнатных квартир
    }
}
