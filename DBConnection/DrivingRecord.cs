using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBConnection
{
    public class DrivingRecord
    {
        public int RecordNumber { get; set; }
        public DateTime Date { get; set; }
        public string DayOfWeek { get; set; }
        public string Time { get; set; }
        public int InstructorId { get; set; }
        public string Instructor { get; set; }
        public int StudentId { get; set; }
        public string Student { get; set; }
        
    }
}
