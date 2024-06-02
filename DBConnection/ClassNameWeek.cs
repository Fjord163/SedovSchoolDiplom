using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBConnection
{
    public class ClassNameWeek
    {
        public ClassNameWeek(int id, string numberWeek, DateTime startWeek, DateTime endWeek)
        {
            Id = id;
            NumberWeek = numberWeek;
            StartWeek = startWeek;
            EndWeek = endWeek;
        }
        public string DisplayText => $"{NumberWeek} ({StartWeek.ToString("d")} - {EndWeek.ToString("d")})";
        public int Id { get; set; }
        public string NumberWeek { get; set; }
        public DateTime StartWeek { get; set; }
        public DateTime EndWeek { get; set; }
    }
}
