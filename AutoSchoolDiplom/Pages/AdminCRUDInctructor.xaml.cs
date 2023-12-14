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

    public partial class AdminCRUDInctructor : Page
    {
        public AdminCRUDInctructor()
        {
            InitializeComponent();

            BindingLvInstructors();

            NameUser.Text = Connection.users.FirstName + " " + Connection.users.LastName + " " + Connection.users.Patronymic;

        }

        private void Filter()
        {
            string searchString = tbSearch.Text.Trim();

            var view = CollectionViewSource.GetDefaultView(lvInstructor.ItemsSource);
            view.Filter = new Predicate<object>(o =>
            {
                FullInfoInstructor instructor = o as FullInfoInstructor;
                if (instructor == null) { return false; }

                bool isVisible = true;
                if (searchString.Length > 0)
                {
                    isVisible = instructor.FirstName.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) != -1 ||
                        instructor.LastName.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) != -1 ||
                        instructor.Patronymic.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) != -1;
                }
                return isVisible;
            });
        }

        public void BindingLvInstructors()
        {
            Binding binding = new Binding();
            binding.Source = Connection.infoInstructors;
            lvInstructor.SetBinding(ItemsControl.ItemsSourceProperty, binding);
            Connection.SelectInfoInstructors();
        }

        private void btnTransitionInstructor_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Вы уже находитесь на данной странице");
        }

        private void btnTransitionLecturer_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AdminCRUDLecturer());
            Connection.infoInstructors.Clear();
        }

        private void btnTransitionStudent_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AdminCRUDPage());
            Connection.infoInstructors.Clear();
        }

        private void btnSignOut_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new EntryPage());
            Connection.infoInstructors.Clear();
        }

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            Filter();
        }

        public void ClearingInformationElements()
        {
            lvInstructor.SelectedItem = null;
            tbLogin.Clear();
            tbPass.Clear();
            tbFirstName.Clear();
            tbLastName.Clear();
            tbPatronymic.Clear();
            tbPhone.Clear();
            tbEmail.Clear();
            tbDrivingExperience.Clear();
            dpDateBirth.Text = null;
            dpDateEmployment.Text = null;
        }

        public void InsertInstructorInfo(FullInfoInstructor instructor)
        {
            var login = tbLogin.Text.Trim();
            var password = tbPass.Text.Trim();
            var firstName = tbFirstName.Text.Trim();
            var lastName = tbLastName.Text.Trim();
            var patronymic = tbPatronymic.Text.Trim();
            var phone = tbPhone.Text.Trim();
            var email = tbEmail.Text.Trim();
            var drivingExperience = tbDrivingExperience.Text.Trim();
            var birth = dpDateBirth.SelectedDate;
            var dateEmployment = dpDateBirth.SelectedDate;
            string role = "Инструктор";

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
                int instructorId = instructor.Id = (int)result;

                cmd = Connection.GetCommand("insert into \"Instructor\" (\"Id\", \"DateEmployment\", \"DrivingExperience\") " +
                    "values (@id, @dateEmployment, @drivingExperience)");
                cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, instructorId);
                cmd.Parameters.AddWithValue("@dateEmployment", NpgsqlDbType.Date, dateEmployment);
                cmd.Parameters.AddWithValue("@drivingExperience", NpgsqlDbType.Varchar, drivingExperience);
                result = cmd.ExecuteNonQuery();

                if (result == null)
                {
                    MessageBox.Show("Данные не добавлены");
                }
                else
                {
                    MessageBox.Show("Данные добавлены");
                }
            }
        }

        private void DeleteInstructor_Click(object sender, RoutedEventArgs e)
        {
            FullInfoInstructor instructor = lvInstructor.SelectedItem as FullInfoInstructor;

            NpgsqlCommand cmd = Connection.GetCommand("delete from \"Instructor\" where \"Id\" = @id returning \"Id\"");
            cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, instructor.Id);
            cmd.Parameters.AddWithValue("@dateEmployment", NpgsqlDbType.Date, instructor.DateEmployment);
            cmd.Parameters.AddWithValue("@drivingExperience", NpgsqlDbType.Varchar, instructor.DrivingExperience);
            var result = cmd.ExecuteScalar();
            if (result != null)
            {
                int instructorId = instructor.Id = (int)result;

                cmd = Connection.GetCommand("DELETE FROM \"User\" WHERE \"Id\" = @id");
                cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, instructorId);
                cmd.Parameters.AddWithValue("@login", NpgsqlDbType.Varchar, instructor.Login);
                cmd.Parameters.AddWithValue("@password", NpgsqlDbType.Varchar, instructor.Password);
                cmd.Parameters.AddWithValue("@firstName", NpgsqlDbType.Varchar, instructor.FirstName);
                cmd.Parameters.AddWithValue("@lastName", NpgsqlDbType.Varchar, instructor.LastName);
                cmd.Parameters.AddWithValue("@patronymic", NpgsqlDbType.Varchar, instructor.Patronymic);
                cmd.Parameters.AddWithValue("@phone", NpgsqlDbType.Varchar, instructor.Phone);
                cmd.Parameters.AddWithValue("@email", NpgsqlDbType.Varchar, instructor.Email);
                cmd.Parameters.AddWithValue("@dateBirth", NpgsqlDbType.Date, instructor.DateBirth);
                cmd.Parameters.AddWithValue("@role", NpgsqlDbType.Varchar, instructor.Role);
                result = cmd.ExecuteNonQuery();
                if (result != null)
                {
                    Connection.infoInstructors.Remove(lvInstructor.SelectedItem as FullInfoInstructor);
                    Connection.infoInstructors.Clear();
                    ClearingInformationElements();
                    BindingLvInstructors();
                }
                if (result == null)
                {
                    MessageBox.Show("Данные не удалены");
                }
                else
                {
                    MessageBox.Show("Данные удалены");
                }
            }
        }

        private void AddInstructor_Click(object sender, RoutedEventArgs e)
        {
            FullInfoInstructor infoInstructor = new FullInfoInstructor();
            InsertInstructorInfo(infoInstructor);
            ClearingInformationElements();
            Connection.infoInstructors.Clear();
            BindingLvInstructors();
        }

        private void UpdateInstructor_Click(object sender, RoutedEventArgs e)
        {
            int InstructorId = (lvInstructor.SelectedItem as FullInfoInstructor).Id;

            string login = tbLogin.Text.Trim();
            string password = tbPass.Text.Trim();
            string firstName = tbFirstName.Text.Trim();
            string lastName = tbLastName.Text.Trim();
            string patronymic = tbPatronymic.Text.Trim();
            string phone = tbPhone.Text.Trim();
            string email = tbEmail.Text.Trim();
            string drivingExperience = tbDrivingExperience.Text.Trim();
            var birth = dpDateBirth.SelectedDate;
            var dateEmployment = dpDateBirth.SelectedDate;

            NpgsqlCommand cmd = Connection.GetCommand("UPDATE \"Instructor\" SET \"DateEmployment\"= @dateEmployment, \"DrivingExperience\" = @drivingExperience where \"Id\" = @id");
            cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, InstructorId);
            cmd.Parameters.AddWithValue("@dateEmployment", NpgsqlDbType.Date, dateEmployment);
            cmd.Parameters.AddWithValue("@drivingExperience", NpgsqlDbType.Varchar, drivingExperience);
            var result = cmd.ExecuteNonQuery();
            if (result != 0)
            {
                cmd = Connection.GetCommand("UPDATE \"User\" SET \"Login\"= @login, \"Password\"= @password, \"FirstName\"= @firstName, \"LastName\"= @lastName," +
                    "\"Patronymic\"= @patronymic, \"Phone\"= @phone, \"Email\"= @email, \"DateBirth\"= @birth where \"Id\" = @id");
                cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, InstructorId);
                cmd.Parameters.AddWithValue("@login", NpgsqlDbType.Varchar, login);
                cmd.Parameters.AddWithValue("@password", NpgsqlDbType.Varchar, password);
                cmd.Parameters.AddWithValue("@firstName", NpgsqlDbType.Varchar, firstName);
                cmd.Parameters.AddWithValue("@lastName", NpgsqlDbType.Varchar, lastName);
                cmd.Parameters.AddWithValue("@patronymic", NpgsqlDbType.Varchar, patronymic);
                cmd.Parameters.AddWithValue("@phone", NpgsqlDbType.Varchar, phone);
                cmd.Parameters.AddWithValue("@email", NpgsqlDbType.Varchar, email);
                cmd.Parameters.AddWithValue("@birth", NpgsqlDbType.Date, birth);
                result = cmd.ExecuteNonQuery();
            }
            if (result != 0)
            {
                MessageBox.Show("Данные обновлены");
            }
            else
            {
                MessageBox.Show("Данные не обновлены");
            }

            ClearingInformationElements();
            Connection.infoInstructors.Clear();
            BindingLvInstructors();
        }

        private void DeselectSelection_Click(object sender, RoutedEventArgs e)
        {
            ClearingInformationElements();
        }

        private void lvInstructor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvInstructor.SelectedItem != null)
            {
                tbLogin.Text = (lvInstructor.SelectedItem as FullInfoInstructor).Login.ToString();
                tbPass.Text = (lvInstructor.SelectedItem as FullInfoInstructor).Password.ToString();
                tbFirstName.Text = (lvInstructor.SelectedItem as FullInfoInstructor).FirstName.ToString();
                tbLastName.Text = (lvInstructor.SelectedItem as FullInfoInstructor).LastName.ToString();
                tbPatronymic.Text = (lvInstructor.SelectedItem as FullInfoInstructor).Patronymic.ToString();
                tbPhone.Text = (lvInstructor.SelectedItem as FullInfoInstructor).Phone.ToString();
                tbEmail.Text = (lvInstructor.SelectedItem as FullInfoInstructor).Email.ToString();
                tbDrivingExperience.Text = (lvInstructor.SelectedItem as FullInfoInstructor).DrivingExperience.ToString();
                dpDateBirth.Text = (lvInstructor.SelectedItem as FullInfoInstructor).DateBirth.ToString();
                dpDateEmployment.Text = (lvInstructor.SelectedItem as FullInfoInstructor).DateEmployment.ToString();
            }
        }
    }
}
