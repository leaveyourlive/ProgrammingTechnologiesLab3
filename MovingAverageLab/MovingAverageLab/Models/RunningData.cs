using System;

namespace RunningApp.Models
{
    public class RunningData
    {
        public DateTime Date { get; set; }
        public double DistanceKm { get; set; }
        public int DurationMinutes { get; set; }
        public double AvgSpeedKmph { get; set; }
        public double MaxSpeedKmph { get; set; }
        public int AvgPulse { get; set; }

        public bool IsWeekend
        {
            get { return Date.DayOfWeek == DayOfWeek.Saturday || Date.DayOfWeek == DayOfWeek.Sunday; }
        }
    }
}