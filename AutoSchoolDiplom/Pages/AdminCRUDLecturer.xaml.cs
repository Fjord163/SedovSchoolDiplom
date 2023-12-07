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

    public partial class AdminCRUDLecturer : Page
    {
        public AdminCRUDLecturer()
        {
            InitializeComponent();

            BindingLvLecturers();
            NameUser.Text = Connection.users.FirstName + " " + Connection.users.LastName + " " + Connection.users.Patronymic;
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
    }
}
