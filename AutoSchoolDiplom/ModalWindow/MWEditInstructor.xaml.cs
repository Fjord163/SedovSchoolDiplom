using DBConnection;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AutoSchoolDiplom.ModalWindow
{
    /// <summary>
    /// Логика взаимодействия для MWEditInstructor.xaml
    /// </summary>
    public partial class MWEditInstructor : Window
    {
        private FullInfoInstructor _fullinfoInstructor;

        public MWEditInstructor(FullInfoInstructor fullInfoInstructor)
        {
            InitializeComponent();

            _fullinfoInstructor = fullInfoInstructor;
            DataContext = fullInfoInstructor;
        }

        private void btnUpdateUser_Click(object sender, RoutedEventArgs e)
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
            var password = tbPass.Text.Trim();
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            var birth = dpDateBirth.SelectedDate;
            var dateEmployment = dpDateEmployment.SelectedDate;
            var drivingExperience = tbDrivingExperience.Text.Trim();

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
                NpgsqlCommand cmd = Connection.GetCommand("UPDATE \"Instructor\" SET \"DateEmployment\"= @dateEmployment, \"DrivingExperience\" = @drivingExperience where \"Id\" = @id");
                cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, _fullinfoInstructor.Id);
                cmd.Parameters.AddWithValue("@dateEmployment", NpgsqlDbType.Date, dateEmployment);
                cmd.Parameters.AddWithValue("@drivingExperience", NpgsqlDbType.Varchar, _fullinfoInstructor.DrivingExperience);
                var result = cmd.ExecuteNonQuery();
                if (result != 0)
                {
                    cmd = Connection.GetCommand("UPDATE \"User\" SET \"Login\"= @login, \"Password\"= @password, \"FirstName\"= @firstName, \"LastName\"= @lastName," +
                        "\"Patronymic\"= @patronymic, \"Phone\"= @phone, \"Email\"= @email, \"DateBirth\"= @birth where \"Id\" = @id");
                    cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, _fullinfoInstructor.Id);
                    cmd.Parameters.AddWithValue("@login", NpgsqlDbType.Varchar, _fullinfoInstructor.Login);
                    cmd.Parameters.AddWithValue("@password", NpgsqlDbType.Varchar, hashedPassword);
                    cmd.Parameters.AddWithValue("@firstName", NpgsqlDbType.Varchar, _fullinfoInstructor.FirstName);
                    cmd.Parameters.AddWithValue("@lastName", NpgsqlDbType.Varchar, _fullinfoInstructor.LastName);
                    cmd.Parameters.AddWithValue("@patronymic", NpgsqlDbType.Varchar, _fullinfoInstructor.Patronymic);
                    cmd.Parameters.AddWithValue("@phone", NpgsqlDbType.Varchar, _fullinfoInstructor.Phone);
                    cmd.Parameters.AddWithValue("@email", NpgsqlDbType.Varchar, _fullinfoInstructor.Email);
                    cmd.Parameters.AddWithValue("@birth", NpgsqlDbType.Date, birth);
                    result = cmd.ExecuteNonQuery();
                }
                if (result != 0) { MessageBox.Show("Данные обновлены"); }
                else { MessageBox.Show("Данные не обновлены"); }
            }
            catch
            {
                MessageBox.Show("Заполните поля");
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void tbPhone_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1) || tbPhone.Text.Length >= 11)
            {
                e.Handled = true;
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
