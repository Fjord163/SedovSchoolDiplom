using DBConnection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Логика взаимодействия для AccountStudent.xaml
    /// </summary>
    public partial class AccountStudent : Page
    {
        public CLassUser _classUser;
        public FullInfoStudent FullInfoStudent { get; set; }
        public AccountStudent(CLassUser authorizedStudent)
        {
            InitializeComponent();
            _classUser = authorizedStudent;
            DataContext = authorizedStudent;
            LoadStudentInfo();
        }
        private void LoadStudentInfo()
        {
            var students = Connection.GetFullInfoStudents();
            FullInfoStudent = students.FirstOrDefault(s => s.Id == _classUser.Id);

            if (FullInfoStudent != null)
            {
                DataContext = FullInfoStudent;
            }
            else
            {
                MessageBox.Show("Информация о студенте не найдена");
            }
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        private void btnTransitionSchedulePage(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ScheduleStudent(_classUser));
        }

        private void btnTransitionAccountPage(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Вы уже находитесь на данной странице");
        }

        private void btnTransmitionRecordDrivingPage(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RecordDriving(_classUser));
        }

        private void btnTransmitionStudentRecordsDrivingPage(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new StudentRecordsDriving(_classUser));
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Connection.users = null;

            NavigationService.Navigate(new EntryPage());
        }
    }
}
