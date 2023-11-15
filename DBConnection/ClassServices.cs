using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBConnection
{
    public class ClassServices
    {
        public ClassServices(int service, int order, int quantity)
        {
            Service = service;
            Order = order;
            Quantity = quantity;
        }

        public int Service { get; set; }
        public int Order { get; set; }
        public int Quantity { get; set; }
    }
}
