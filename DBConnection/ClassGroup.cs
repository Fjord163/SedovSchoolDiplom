using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBConnection
{
    public class ClassGroup
    {
        public ClassGroup(int id, string numberGroup, int lecturer)
        {
            Id = id;
            NumberGroup = numberGroup;
            Lecturer = lecturer;
        }

        public int Id { get; set; }
        public string NumberGroup { get; set; }
        public int Lecturer { get; set; }

    }
}
