using DBConnection;
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

        private void DeleteInstructor_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
