using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
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
using System.Xml;
using DBConnection;
using Npgsql;

namespace AutoSchoolDiplom.Pages
{
    public partial class ScheduleStudent : Page
    {

        private ScheduleViewModel _viewModel;
        public ScheduleStudent(CLassUser authorizedStudent)
        {
            InitializeComponent();
            _viewModel = new ScheduleViewModel();
            DataContext = _viewModel;
            _viewModel.AuthorizedStudent = authorizedStudent;
        }
        public class ScheduleViewModel : INotifyPropertyChanged
        {
            private ClassNameWeek _selectedWeek;
            private CLassUser _authorizedStudent;
            private string _studentGroup;

            public ObservableCollection<ClassNameWeek> Weeks { get; set; }
            public ObservableCollection<TimeTable> MondaySchedule { get; set; }
            public ObservableCollection<TimeTable> TuesdaySchedule { get; set; }
            public ObservableCollection<TimeTable> WednesdaySchedule { get; set; }
            public ObservableCollection<TimeTable> ThursdaySchedule { get; set; }
            public ObservableCollection<TimeTable> FridaySchedule { get; set; }
            public ObservableCollection<TimeTable> SaturdaySchedule { get; set; }

           

            public ScheduleViewModel()
            {
                Weeks = new ObservableCollection<ClassNameWeek>();
                MondaySchedule = new ObservableCollection<TimeTable>();
                TuesdaySchedule = new ObservableCollection<TimeTable>();
                WednesdaySchedule = new ObservableCollection<TimeTable>();
                ThursdaySchedule = new ObservableCollection<TimeTable>();
                FridaySchedule = new ObservableCollection<TimeTable>();
                SaturdaySchedule = new ObservableCollection<TimeTable>();
                LoadWeeks();
            }

            public CLassUser AuthorizedStudent
            {
                get { return _authorizedStudent; }
                set
                {
                    _authorizedStudent = value;
                    OnPropertyChanged();
                    LoadFilteredTimetable();
                    LoadStudentGroup();
                }
            }
            public string StudentGroup
            {
                get { return _studentGroup; }
                set
                {
                    _studentGroup = value;
                    OnPropertyChanged();
                }
            }

            private void LoadWeeks()
            {
                DateTime startOfYear = new DateTime(DateTime.Now.Year, 1, 1);
                DateTime endOfYear = new DateTime(DateTime.Now.Year, 12, 31);
                int weekNumber = 1;
                DateTime currentStart = startOfYear;

                while (currentStart <= endOfYear)
                {
                    DateTime currentEnd = currentStart.AddDays(6);
                    if (currentEnd > endOfYear)
                    {
                        currentEnd = endOfYear;
                    }

                    Weeks.Add(new ClassNameWeek(weekNumber, currentStart, currentEnd));

                    weekNumber++;
                    currentStart = currentEnd.AddDays(1);
                }
            }
            public ClassNameWeek SelectedWeek
            {
                get { return _selectedWeek; }
                set
                {
                    _selectedWeek = value;
                    OnPropertyChanged();
                    LoadFilteredTimetable();
                }
            }
            private void LoadFilteredTimetable()
            {
                try
                {
                    if (SelectedWeek != null && AuthorizedStudent != null)
                    {
                        string query = "SELECT tl.\"Id\", tl.\"Weekday\", tl.\"Time\", tl.\"Group\", g.\"NumberGroup\", tl.\"Office\", tl.\"Date\" " +
                                       "FROM \"TimetableTheory\" tl " +
                                       "JOIN \"Group\" g ON tl.\"Group\" = g.\"Id\" " +
                                       "JOIN \"StudentGroup\" sg ON g.\"Id\" = sg.\"Group\" " +
                                       "WHERE sg.\"Student\" = @StudentId AND tl.\"Date\" BETWEEN @StartDate AND @EndDate";

                        NpgsqlCommand command = Connection.GetCommand(query);
                        command.Parameters.AddWithValue("StartDate", SelectedWeek.StartDate);
                        command.Parameters.AddWithValue("EndDate", SelectedWeek.EndDate);
                        command.Parameters.AddWithValue("StudentId", AuthorizedStudent.Id);

                        var reader = command.ExecuteReader();
                        ClearSchedules();
                        while (reader.Read())
                        {
                            var timetable = new TimeTable
                            {
                                Id = reader.GetInt32(0),
                                Weekday = reader.GetString(1),
                                Time = reader.GetTimeSpan(2),
                                Group = reader.GetInt32(3),
                                NumberGroup = reader.GetString(4),
                                Office = reader.GetString(5),
                                Date = reader.GetDateTime(6)
                            };

                            switch (timetable.Weekday)
                            {
                                case "Понедельник":
                                    MondaySchedule.Add(timetable);
                                    break;
                                case "Вторник":
                                    TuesdaySchedule.Add(timetable);
                                    break;
                                case "Среда":
                                    WednesdaySchedule.Add(timetable);
                                    break;
                                case "Четверг":
                                    ThursdaySchedule.Add(timetable);
                                    break;
                                case "Пятница":
                                    FridaySchedule.Add(timetable);
                                    break;
                                case "Суббота":
                                    SaturdaySchedule.Add(timetable);
                                    break;
                            }
                        }
                        reader.Close();
                    }
                    else
                    {
                        ClearSchedules();
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Ошибка загрузки расписания: " + ex.Message);
                }
            }

            private void ClearSchedules()
            {
                MondaySchedule.Clear();
                TuesdaySchedule.Clear();
                WednesdaySchedule.Clear();
                ThursdaySchedule.Clear();
                FridaySchedule.Clear();
                SaturdaySchedule.Clear();
            }
            private void LoadStudentGroup()
            {
                try
                {
                    NpgsqlCommand cmd = Connection.GetCommand("SELECT \"Group\".\"NumberGroup\" FROM \"StudentGroup\" " +
                                                              "JOIN \"Group\" ON \"StudentGroup\".\"Group\" = \"Group\".\"Id\" " +
                                                              "WHERE \"StudentGroup\".\"Student\" = @StudentId");
                    cmd.Parameters.AddWithValue("StudentId", AuthorizedStudent.Id);
                    NpgsqlDataReader result = cmd.ExecuteReader();

                    if (result.HasRows)
                    {
                        result.Read();
                        StudentGroup = result.GetString(0);
                    }
                    result.Close();
                }
                catch
                {
                    MessageBox.Show("Ошибка загрузки группы студента: ");
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }



        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void btnTransitionAccountPage(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AccountStudent(_viewModel.AuthorizedStudent));
        }

        private void btnTransitionSchedulePage(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Вы уже находитесь на данной странице");
        }

        private void btnTransmitionRecordDrivingPage(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RecordDriving(_viewModel.AuthorizedStudent));
        }
    }
}
