using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBConnection
{
    public class ClassLecturer
    {
        public ClassLecturer(int id, string firstName, string lastName, string patronymic, string password, string phone, string email)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Patronymic = patronymic;
            Password = password;
            Phone = phone;
            Email = email;
        }

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

    }
}
