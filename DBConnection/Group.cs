using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBConnection
{
    public class Group
    {
        public Group(int id, string numberGroup)
        {
            Id = id;
            NumberGroup = numberGroup;
        }

        public int Id { get; set; }
        public string NumberGroup { get; set; }
    }
}
