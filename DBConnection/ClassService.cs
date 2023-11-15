using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBConnection
{
    public class ClassService
    {
        public ClassService(int id, string name, string description, string price)
        {
            Id = id;
            Name = name;
            Description = description;
            Price = price;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Price { get; set; }
    }
}
