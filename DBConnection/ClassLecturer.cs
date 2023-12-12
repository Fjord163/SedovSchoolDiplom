using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBConnection
{
    public class ClassLecturer
    {
        public ClassLecturer(int id, DateTime dateEmployment)
        {
            Id = id;
            DateEmployment = dateEmployment;
        }

        public int Id { get; set; }
        public DateTime DateEmployment { get; set; }
    }
}
