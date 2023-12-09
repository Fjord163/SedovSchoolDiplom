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

            DataContext = this;

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

        private void DeleteLecturer_Click(object sender, RoutedEventArgs e)
        {
            
           
        }

        private void AddLecturer_Click(object sender, RoutedEventArgs e)
        {
            var login = tbLogin.Text.Trim();
            var password = tbPass.Text.Trim();
            var firstName = tbFirstName.Text.Trim();
            var lastName = tbLastName.Text.Trim();
            var patronymic = tbPatronymic.Text.Trim();
            var phone = tbPhone.Text.Trim();
            var email = tbEmail.Text.Trim();
            var birth = tbDateBirth.SelectedDate;
            string role = "Лектор";
            

            Connection.InsertUsers(new User(login, password, firstName, lastName, patronymic, phone, email, birth, role));
            Connection.infoLecturers.Clear();
            BindingLvLecturers();
        }
    }
}
