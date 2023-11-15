using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBConnection
{
    public class ClassStudentInstructor
    {
        public ClassStudentInstructor(int student, int instructor)
        {
            Student = student;
            Instructor = instructor;
        }

        public int Student { get; set; }
        public int Instructor { get; set; }
    }
}
