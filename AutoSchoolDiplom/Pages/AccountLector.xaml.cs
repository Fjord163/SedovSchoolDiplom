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
    /// <summary>
    /// Логика взаимодействия для AccountLector.xaml
    /// </summary>
    public partial class AccountLector : Page
    {
        public CLassUser _classUser;
        public AccountLector(CLassUser authorizedLecturer)
        {
            InitializeComponent();
            _classUser = authorizedLecturer;
            DataContext = authorizedLecturer;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        private void btnTransitionAccountPage(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Вы уже находитесь на данной странице");
        }

        private void btnTransitionSchedulePage(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ScheduleLector(_classUser));
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Connection.users = null;

            NavigationService.Navigate(new EntryPage());
        }
    }
}
