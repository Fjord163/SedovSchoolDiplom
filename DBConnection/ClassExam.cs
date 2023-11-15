using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBConnection
{
    public class ClassExam
    {
        public ClassExam(int id, DateTime date, string result, int student)
        {
            Id = id;
            Date = date;
            Result = result;
            Student = student;
        }

        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Result { get; set; }
        public int Student { get; set; }
    }
}
