using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBConnection
{
    public class ClassOrder
    {
        public ClassOrder(int id, string price, string statusPayment, int student)
        {
            Id = id;
            Price = price;
            StatusPayment = statusPayment;
            Student = student;
        }

        public int Id { get; set; }
        public string Price { get; set; }
        public string StatusPayment { get; set; }
        public int Student { get; set; }
    }
}
