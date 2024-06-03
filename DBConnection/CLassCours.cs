using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBConnection
{
    public class CLassCours
    {
        public CLassCours() { }
        public CLassCours(int id, string category, string theoryHours, string drivingHours)
        {
            Id = id;
            Category = category;
            TheoryHours = theoryHours;
            DrivingHours = drivingHours;
        }

        public int Id { get; set; }
        public string Category { get; set; }
        public string TheoryHours { get; set; }
        public string DrivingHours { get; set; }
    }
}
