using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBConnection
{
    public class ClassDrivingLesson
    {
        public ClassDrivingLesson(int id, DateTime dateTime, int student, string car)
        {
            Id = id;
            DateTime = dateTime;
            Student = student;
            Car = car;
        }
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public int Student { get; set; }
        public string Car { get; set; }
    }
}
