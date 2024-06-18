using DBConnection;
using Npgsql;
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
    /// Логика взаимодействия для InstructorRecordsDriving.xaml
    /// </summary>
    public partial class InstructorRecordsDriving : Page
    {
        public CLassUser _classUser;
        public InstructorRecordsDriving(CLassUser authorizedInstructor)
        {
            InitializeComponent();
            _classUser = authorizedInstructor;
            LoadSchedule();
        }
        private void LoadSchedule()
        {
            List<DrivingRecord> drivingRecords = new List<DrivingRecord>();

            try
            {
                NpgsqlCommand cmd = Connection.GetCommand("SELECT s.\"Date\", s.\"Time\", u.\"Id\" AS \"StudentId\", u.\"FirstName\" || ' ' || u.\"LastName\" AS \"Student\" " +
                        "FROM \"Schedule\" s " +
                        "INNER JOIN \"User\" u ON s.\"StudentId\" = u.\"Id\" " +
                        "INNER JOIN \"Instructor\" i ON s.\"InstructorId\" = i.\"Id\" " +
                        "WHERE i.\"Id\" = @InstructorId");
                cmd.Parameters.AddWithValue("@InstructorId", _classUser.Id);
                NpgsqlDataReader result = cmd.ExecuteReader();
                int recordCounter = 1;
                while (result.Read())
                {
                    drivingRecords.Add(new DrivingRecord
                    {
                        RecordNumber = recordCounter++,
                        Date = result.GetDateTime(0),
                        DayOfWeek = char.ToUpper(result.GetDateTime(0).ToString("dddd")[0]) + result.GetDateTime(0).ToString("dddd").Substring(1),
                        Time = result.GetString(1),
                        StudentId = result.GetInt32(2),
                        Student = result.GetString(3)
                    });
                }
                lvRecordsInstructor.ItemsSource = drivingRecords;
                result.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке данных: " + ex.Message);
            }
        }
        private void btnTransitioAccountnPage(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AccountInstructor(_classUser));
        }

        private void btnTransmitionRecordsPage(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Вы уже находитесь на данной странице");
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Connection.users = null;

            NavigationService.Navigate(new EntryPage());
        }
    }
}
