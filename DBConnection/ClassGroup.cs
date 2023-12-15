using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBConnection
{
    public class ClassGroup
    {
        public ClassGroup() { }
        public ClassGroup(int id, string numberGroup, int lecturer, string firstName, string lastName, string patronymic)
        {
            Id = id;
            NumberGroup = numberGroup;
            Lecturer = lecturer;
            FirstName = firstName;
            LastName = lastName;
            Patronymic = patronymic;
        }

        public int Id { get; set; }
        public string NumberGroup { get; set; }
        public int Lecturer { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }

    }
}
