using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBConnection
{
    public class ClassTimetableLecture
    {
        public ClassTimetableLecture(int id, int numberWeek, string weekday, TimeSpan time, int lecturer)
        {
            Id = id;
            NumberWeek = numberWeek;
            Weekday = weekday;
            Time = time;
            Lecturer = lecturer;
        }

        public int Id { get; set; }
        public int NumberWeek { get; set; }
        public string Weekday { get; set; }
        public TimeSpan Time { get; set; }
        public int Lecturer { get; set; }

    }
}
