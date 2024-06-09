using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBConnection
{
    public class ClassNameWeek
    {
        public ClassNameWeek( int weekNumber, DateTime startDate, DateTime endDate)
        {
            WeekNumber = weekNumber;
            StartDate = startDate;
            EndDate = endDate;
        }
        public string DisplayText
        {
            get
            {
                string displayText = $"Неделя {WeekNumber} ({StartDate.ToString("d")} - {EndDate.ToString("d")})";
                return IsCurrentWeek ? "* " + displayText : displayText;
            }
        }
            
        public int WeekNumber { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsCurrentWeek { get; set; }
    }
}
