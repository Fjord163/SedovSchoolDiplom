using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBConnection
{
    public class SheduleItem
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Time { get; set; }
        public bool IsBooked { get; set; }
        public int StudentId { get; set; }
        public int InstructorId { get; set; }
    }
}
