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
    /// Логика взаимодействия для AccountInstructor.xaml
    /// </summary>
    public partial class AccountInstructor : Page
    {
        public CLassUser _classUser;
        public FullInfoInstructor FullInfoInstructor { get; set; }

        public AccountInstructor(CLassUser authorizedInstructor)
        {
            InitializeComponent();
            _classUser = authorizedInstructor;
            DataContext = authorizedInstructor;
            LoadStudentInfo();
        }
        private void LoadStudentInfo()
        {
            var instructors = Connection.GetFullInfoInstructor();
            FullInfoInstructor = instructors.FirstOrDefault(s => s.Id == _classUser.Id);

            if (FullInfoInstructor != null)
            {
                DataContext = FullInfoInstructor;
            }
            else
            {
                MessageBox.Show("Информация о инструкторе не найдена");
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

        private void btnTransitionAccountPage(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Вы уже находитесь на данной странице");
        }

        private void btnTransmitionRecordsPage(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new InstructorRecordsDriving(_classUser));
        }
    }
}
