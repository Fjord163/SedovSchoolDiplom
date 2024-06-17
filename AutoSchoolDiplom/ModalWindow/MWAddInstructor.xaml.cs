using DBConnection;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BCrypt.Net;

namespace AutoSchoolDiplom.ModalWindow
{
    /// <summary>
    /// Логика взаимодействия для MWAddInstructor.xaml
    /// </summary>
    public partial class MWAddInstructor : Window
    {
        public MWAddInstructor()
        {
            InitializeComponent();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        public void InsertInstructorInfo(FullInfoInstructor instructor)
        {
           
            if (string.IsNullOrWhiteSpace(tbLogin.Text) ||
                string.IsNullOrWhiteSpace(tbPass.Text) ||
                string.IsNullOrWhiteSpace(tbFirstName.Text) ||
                string.IsNullOrWhiteSpace(tbLastName.Text) ||
                string.IsNullOrWhiteSpace(tbPhone.Text) ||
                string.IsNullOrWhiteSpace(tbDrivingExperience.Text) ||
                string.IsNullOrWhiteSpace(tbEmail.Text) ||
                dpDateBirth.SelectedDate == null ||
                dpDateEmployment.SelectedDate == null)
            {
                MessageBox.Show("Не все обязательные поля заполнены.");
                return;
            }
            var login = tbLogin.Text.Trim();
            var password = tbPass.Text.Trim();
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            var firstName = tbFirstName.Text.Trim();
            var lastName = tbLastName.Text.Trim();
            var patronymic = tbPatronymic.Text.Trim();
            var phone = tbPhone.Text.Trim();
            var email = tbEmail.Text.Trim();
            var drivingExperience = tbDrivingExperience.Text.Trim();
            var birth = dpDateBirth.SelectedDate;
            var dateEmployment = dpDateBirth.SelectedDate;
            string role = "Инструктор";

            if (!birth.HasValue)
            {
                MessageBox.Show("Укажите дату рождения.");
                return;
            }

            if (!int.TryParse(drivingExperience, out var drivingExperienceYears))
            {
                MessageBox.Show("Стаж вождения должен быть числом.");
                return;
            }
            var minimumDrivingExperience = 3;

            if (drivingExperienceYears < minimumDrivingExperience)
            {
                MessageBox.Show($"Стаж вождения не может быть меньше {minimumDrivingExperience} лет. Проверьте данные.");
                return;
            }
            var minimumBirthYear = DateTime.Now.Year - drivingExperienceYears - 18;
            if (birth.Value.Year > minimumBirthYear)
            {
                MessageBox.Show($"Год рождения неверно указан, минимальный год рождения: {minimumBirthYear}.");
                return;
            }
            try
            {
                NpgsqlCommand cmd = Connection.GetCommand("insert into \"User\" (\"Login\", \"Password\", \"FirstName\", \"LastName\", \"Patronymic\", \"Phone\", \"Email\", \"DateBirth\", \"Role\")" +
                    "values (@login, @password, @firstName, @lastName, @patronymic, @phone, @email, @dateBirth, @role) returning \"Id\"");
                cmd.Parameters.AddWithValue("@login", NpgsqlDbType.Varchar, login);
                cmd.Parameters.AddWithValue("@password", NpgsqlDbType.Varchar, hashedPassword);
                cmd.Parameters.AddWithValue("@firstName", NpgsqlDbType.Varchar, firstName);
                cmd.Parameters.AddWithValue("@lastName", NpgsqlDbType.Varchar, lastName);
                cmd.Parameters.AddWithValue("@patronymic", NpgsqlDbType.Varchar, patronymic);
                cmd.Parameters.AddWithValue("@phone", NpgsqlDbType.Varchar, phone);
                cmd.Parameters.AddWithValue("@email", NpgsqlDbType.Varchar, email);
                cmd.Parameters.AddWithValue("@dateBirth", NpgsqlDbType.Date, birth);
                cmd.Parameters.AddWithValue("@role", NpgsqlDbType.Varchar, role);
                var result = cmd.ExecuteScalar();
                if (result != null)
                {
                    int instructorId = instructor.Id = (int)result;

                    cmd = Connection.GetCommand("insert into \"Instructor\" (\"Id\", \"DateEmployment\", \"DrivingExperience\") values (@id, @dateEmployment, @drivingExperience)");
                    cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, instructorId);
                    cmd.Parameters.AddWithValue("@dateEmployment", NpgsqlDbType.Date, dateEmployment);
                    cmd.Parameters.AddWithValue("@drivingExperience", NpgsqlDbType.Varchar, drivingExperience);
                    result = cmd.ExecuteNonQuery();

                    if (result == null) { MessageBox.Show("Данные не добавлены"); }
                    else { MessageBox.Show("Данные добавлены"); }
                }
            }
            catch
            {
                MessageBox.Show("Заполните поля");
            }
        }
        private void btnCreateUser_Click(object sender, RoutedEventArgs e)
        {
            FullInfoInstructor instructor = new FullInfoInstructor();
            InsertInstructorInfo(instructor);
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void tbPhone_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1) || tbPhone.Text.Length >= 11)
            {
                e.Handled = true;
                return;
            }
        }

        private void tbDrivingExperience_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1) || tbDrivingExperience.Text.Length >= 2)
            {
                e.Handled = true;
            }
        }
    }
}
