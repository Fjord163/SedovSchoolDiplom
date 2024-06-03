using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBConnection
{
    public class FullInfoStudent
    {
        public FullInfoStudent() { }
        public FullInfoStudent(int id, string login, string password, string firstName, string lastName, string patronymic, string phone, string email, DateTime dateBirth, string role, string photo, string category, string theoryHours, string drivingHours, string numberGroup, int student, int group, int cours)
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
            Photo = photo;
            Category = category;
            TheoryHours = theoryHours;
            DrivingHours = drivingHours;
            NumberGroup = numberGroup;
            Student = student;
            Group = group;
            Cours = cours;
        }
        public string DisplayCours => $"{Category} {TheoryHours} ч. {DrivingHours} ч.";
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
        public string Photo { get; set; }
        public string Category { get; set; }
        public string TheoryHours { get; set; }
        public string DrivingHours { get; set; }
        public string NumberGroup { get; set; }
        public int Student { get; set; }
        public int Group { get; set; }
        public int Cours { get; set; }


    }
}
