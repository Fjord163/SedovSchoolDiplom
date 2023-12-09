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


    public partial class AdminCRUDPage : Page
    {
        public AdminCRUDPage()
        {
            InitializeComponent();

            BindingLvStudents();

            NameUser.Text = Connection.users.FirstName + " " + Connection.users.LastName + " " + Connection.users.Patronymic;


        }

        public void BindingLvStudents()
        {
            Binding binding = new Binding();
            binding.Source = Connection.infoStudents;
            lvStudents.SetBinding(ItemsControl.ItemsSourceProperty, binding);
            Connection.SelectInfoStudent();
        }

        private void Filter()
        {
            string searchString = tbSearch.Text.Trim();

            var view = CollectionViewSource.GetDefaultView(lvStudents.ItemsSource);
            view.Filter = new Predicate<object>(o =>
            {
                FullInfoStudent student = o as FullInfoStudent;
                if (student == null) { return false; }

                bool isVisible = true;
                if (searchString.Length > 0)
                {
                    isVisible = student.FirstName.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) != -1 ||
                        student.LastName.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) != -1 ||
                        student.Patronymic.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) != -1 ||
                        student.NumberGroup.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) != -1;
                }
                return isVisible;
            });
        }

        private void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            Filter();
        }

        private void btnTransitionInstructor_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AdminCRUDInctructor());
            Connection.infoStudents.Clear();
        }

        private void btnTransitionLecturer_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AdminCRUDLecturer());
            Connection.infoStudents.Clear();
        }

        private void btnTransitionStudent_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Вы уже находитесь на данной странице");
        }

        private void btnSignOut_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new EntryPage());
            Connection.infoStudents.Clear();
        }

        private void DeleteStudent_Click(object sender, RoutedEventArgs e)
        {
            FullInfoStudent studentInfo = lvStudents.SelectedItem as FullInfoStudent;

           
        }

        private void AddStudent_Click(object sender, RoutedEventArgs e)
        {
            var login = tbLogin.Text.Trim();
            var password = tbPass.Text.Trim();
            var firstName = tbFirstName.Text.Trim();
            var lastName = tbLastName.Text.Trim();
            var patronymic = tbPatronymic.Text.Trim();
            var phone = tbPhone.Text.Trim();
            var email = tbEmail.Text.Trim();
            var birth = tbDateBirth.SelectedDate;
            string role = "Ученик";

            Connection.InsertUsers(new User(login, password, firstName, lastName, patronymic, phone, email, birth, role));

        }
    }
}
