using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBConnection
{
    public class ClassCar
    {
        public ClassCar(string vin, string stamp, string model, string color)
        {
            VIN = vin;
            Stamp = stamp;
            Model = model;
            Color = color;
        }

        public string VIN { get; set; }
        public string Stamp { get; set; }
        public string Model { get; set; }
        public string Color { get; set; }
    }
}
