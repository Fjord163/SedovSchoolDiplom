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

namespace AutoSchoolDiplom.ModalWindow
{
    /// <summary>
    /// Логика взаимодействия для MWAddLecturer.xaml
    /// </summary>
    public partial class MWAddLecturer : Window
    {
        private FullInfoLecturer _fullInfoLecturer;
        public MWAddLecturer()
        {
            InitializeComponent();
        }

        public void InsertLecturerInfo(FullInfoLecturer lecturer)
        {
            try
            {
                var login = tbLogin.Text.Trim();
                var password = tbPass.Text.Trim();
                var firstName = tbFirstName.Text.Trim();
                var lastName = tbLastName.Text.Trim();
                var patronymic = tbPatronymic.Text.Trim();
                var phone = tbPhone.Text.Trim();
                var email = tbEmail.Text.Trim();
                var birth = dpDateBirth.SelectedDate;
                var dateEmployment = dpDateEmployment.SelectedDate;
                string role = "Лектор";

                NpgsqlCommand cmd = Connection.GetCommand("insert into \"User\" (\"Login\", \"Password\", \"FirstName\", \"LastName\", \"Patronymic\", \"Phone\", \"Email\", \"DateBirth\", \"Role\")" +
                     "values (@login, @password, @firstName, @lastName, @patronymic, @phone, @email, @dateBirth, @role) returning \"Id\"");
                cmd.Parameters.AddWithValue("@login", NpgsqlDbType.Varchar, login);
                cmd.Parameters.AddWithValue("@password", NpgsqlDbType.Varchar, password);
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
                    int IdLecturer = lecturer.Id = (int)result;

                    cmd = Connection.GetCommand("insert into \"Lecturer\" (\"Id\", \"DateEmployment\") values (@id, @dateEmployment)");
                    cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, IdLecturer);
                    cmd.Parameters.AddWithValue("@dateEmployment", NpgsqlDbType.Date, dateEmployment);
                    cmd.ExecuteNonQuery();
                }
                if (result == null) { MessageBox.Show("Данные не добавлены"); }
                else { MessageBox.Show("Данные добавлены"); }
            }
            catch
            {
                MessageBox.Show("Заполните поля");
            }
        }

        private void MWCrateUser_Click(object sender, RoutedEventArgs e)
        {
            FullInfoLecturer lecturer = new FullInfoLecturer();
            InsertLecturerInfo(lecturer);
            
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
    }
}
