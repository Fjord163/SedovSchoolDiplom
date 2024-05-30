using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBConnection
{
    public class ScheduleGroupStudent
    {
        public ScheduleGroupStudent(int id, int student, int group, string numberGroup)
        {
            Id = id;
            Student = student;
            Group = group;
            NumberGroup = numberGroup;
        }

        public int Id { get; set; }
        public int Student { get; set; }
        public int Group { get; set; }
        public string NumberGroup { get; set; }
    }
}
