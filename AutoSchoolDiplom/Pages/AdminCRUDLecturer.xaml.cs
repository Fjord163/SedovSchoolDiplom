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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AutoSchoolDiplom.Pages
{

    public partial class AdminCRUDLecturer : Page
    {
        public AdminCRUDLecturer()
        {
            InitializeComponent();

            BindingLvLecturers();

            NameUser.Text = Connection.users.FirstName + " " + Connection.users.LastName + " " + Connection.users.Patronymic;
        }

        private void Filter()
        {
            string searchString = tbSearch.Text.Trim();

            var view = CollectionViewSource.GetDefaultView(lvLecturer.ItemsSource);
            view.Filter = new Predicate<object>(o =>
            {
                FullInfoLecturer lecturer = o as FullInfoLecturer;
                if (lecturer == null) { return false; }

                bool isVisible = true;
                if (searchString.Length > 0)
                {
                    isVisible = lecturer.FirstName.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) != -1 ||
                        lecturer.LastName.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) != -1 ||
                        lecturer.Patronymic.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) != -1;
                }
                return isVisible;
            });
        }

        public void BindingLvLecturers()
        {
            Binding binding = new Binding();
            binding.Source = Connection.infoLecturers;
            lvLecturer.SetBinding(ItemsControl.ItemsSourceProperty, binding);
            Connection.SelectInfoLecturers();
        }

        private void btnTransitionInstructor_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AdminCRUDInctructor());
            Connection.infoLecturers.Clear();
        }

        private void btnTransitionLecturer_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Вы уже находитесь на данной странице");
        }

        private void btnTransitionStudent_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AdminCRUDPage());
            Connection.infoLecturers.Clear();
        }

        private void btnSignOut_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new EntryPage());
            Connection.infoLecturers.Clear();
        }

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            Filter();
        }


        public void InsertLecturerInfo(User user)
        {
            var login = tbLogin.Text.Trim();
            var password = tbPass.Text.Trim();
            var firstName = tbFirstName.Text.Trim();
            var lastName = tbLastName.Text.Trim();
            var patronymic = tbPatronymic.Text.Trim();
            var phone = tbPhone.Text.Trim();
            var email = tbEmail.Text.Trim();
            var birth = tbDateBirth.SelectedDate;
            var dateEmployment = tbDateEmployment.SelectedDate;
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
                int IdLecturer = user.Id = (int)result;

                cmd = Connection.GetCommand("insert into \"Lecturer\" (\"Id\", \"DateEmployment\")" +
                 "values (@id, @dateEmployment)");
                cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, IdLecturer);
                cmd.Parameters.AddWithValue("@dateEmployment", NpgsqlDbType.Date, dateEmployment);
                cmd.ExecuteNonQuery();
            }
            if (result == null)
            {
                MessageBox.Show("Данные не добавлены");
            }
            else
            {
                MessageBox.Show("Данные добавлены");
            }
        }

        private void AddLecturer_Click(object sender, RoutedEventArgs e)
        {
            User user = new User();
            InsertLecturerInfo(user);
            Connection.infoLecturers.Clear();
            BindingLvLecturers();
        }

        private void DeleteLecturer_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
