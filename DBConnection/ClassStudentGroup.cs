using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBConnection
{
    public class ClassStudentGroup
    {
        public ClassStudentGroup(int student, int group)
        {
            Student = student;
            Group = group;
        }

        public int Student { get; set; }
        public int Group { get; set; }
    }
}
