using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBConnection
{
    public class TimeTable
    {
        public int Id { get; set; }
        public string Weekday { get; set; }
        public TimeSpan Time { get; set; }
        public int Group { get; set; }
        public string NumberGroup { get; set; }
        public string Office { get; set;}
        public DateTime Date { get; set; }
    }
}
