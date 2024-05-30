using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoSchoolDiplom
{
    public class StudentComboBoxItem
    {
        public StudentComboBoxItem() {}
        public StudentComboBoxItem(string category, string theoryHours, string drivingHours)
        {
            Category = category;
            TheoryHours = theoryHours;
            DrivingHours = drivingHours;
        }

        public string Category { get; set; }
        public string TheoryHours { get; set; }
        public string DrivingHours { get; set; }
    }
}
