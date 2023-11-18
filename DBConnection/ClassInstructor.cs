using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBConnection
{
    public class ClassInstructor
    {
        public ClassInstructor()
        {
        }

        public ClassInstructor(int id, string firstName, string lastName, string patronymic, string password, string phone, string email, string login)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Patronymic = patronymic;
            Password = password;
            Phone = phone;
            Email = email;
            Login = login;
        }

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Login { get; set; }

    }
}
