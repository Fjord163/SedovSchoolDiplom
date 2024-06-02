using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBConnection
{
    public class CLassUser
    {

        public CLassUser() { }
        public CLassUser(int id, string login, string password, string firstName, string lastName, string patronymic, string phone, string email, DateTime dateBirth, string role, DateTime dateEmployment, string category, int drivingHours, int theoryHours, string numberGroup)
        {
            Id = id;
            Login = login;
            Password = password;
            FirstName = firstName;
            LastName = lastName;
            Patronymic = patronymic;
            Phone = phone;
            Email = email;
            DateBirth = dateBirth;
            Role = role;
            DateEmployment = dateEmployment;
            Category = category;
            DrivingHours = drivingHours;
            TheoryHours = theoryHours;
            NumberGroup = numberGroup;
        }

        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public DateTime DateBirth { get; set; }
        public string Role { get; set; }
        public DateTime DateEmployment { get; set; }
        public string Category { get; set; }  // Новое свойство
        public int DrivingHours { get; set; } // Новое свойство
        public int TheoryHours { get; set; }  // Новое свойство
        public string NumberGroup { get; set; }
    }
}
