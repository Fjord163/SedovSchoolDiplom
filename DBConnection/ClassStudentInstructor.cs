using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBConnection
{
    public class ClassStudentInstructor
    {
        public ClassStudentInstructor(int id, string firstName, string lastName, string patronymic, string password, string phone, string email, string photo, int cours, string login, int student, int instructor)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Patronymic = patronymic;
            Password = password;
            Phone = phone;
            Email = email;
            Photo = photo;
            Cours = cours;
            Login = login;
            Student = student;
            Instructor = instructor;
        }

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Photo { get; set; }
        public int Cours { get; set; }
        public string Login { get; set; }
        public int Student { get; set; }
        public int Instructor { get; set; }
    }
}
