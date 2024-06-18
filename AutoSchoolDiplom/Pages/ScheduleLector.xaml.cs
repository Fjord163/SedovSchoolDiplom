using DBConnection;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace AutoSchoolDiplom.Pages
{
    /// <summary>
    /// Логика взаимодействия для ScheduleLector.xaml
    /// </summary>
    public partial class ScheduleLector : Page
    {
        private ScheduleViewModel _viewModel;
        private CLassUser _user;

        public ScheduleLector(CLassUser authorizedLecturer)
        {
            InitializeComponent();
            _viewModel = new ScheduleViewModel();
            _user = authorizedLecturer;
            DataContext = _viewModel;
            _viewModel.MarkCurrentWeek();
        }

        public class ScheduleViewModel : INotifyPropertyChanged
        {
            private ClassNameWeek _selectedWeek;
            private ClassGroup _selectedGroup;

            public ObservableCollection<ClassNameWeek> Weeks { get; set; }
            public ObservableCollection<ClassGroup> Groups { get; set; }
            public ObservableCollection<TimeTable> MondaySchedule { get; set; }
            public ObservableCollection<TimeTable> TuesdaySchedule { get; set; }
            public ObservableCollection<TimeTable> WednesdaySchedule { get; set; }
            public ObservableCollection<TimeTable> ThursdaySchedule { get; set; }
            public ObservableCollection<TimeTable> FridaySchedule { get; set; }
            public ObservableCollection<TimeTable> SaturdaySchedule { get; set; }

            public ClassGroup SelectedGroup
            {
                get { return _selectedGroup; }
                set
                {
                    _selectedGroup = value;
                    OnPropertyChanged();
                    LoadFilteredTimetable();
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

            public ScheduleViewModel()
            {
                Weeks = new ObservableCollection<ClassNameWeek>();
                Groups = new ObservableCollection<ClassGroup>();
                MondaySchedule = new ObservableCollection<TimeTable>();
                TuesdaySchedule = new ObservableCollection<TimeTable>();
                WednesdaySchedule = new ObservableCollection<TimeTable>();
                ThursdaySchedule = new ObservableCollection<TimeTable>();
                FridaySchedule = new ObservableCollection<TimeTable>();
                SaturdaySchedule = new ObservableCollection<TimeTable>();

                LoadWeeks();
                LoadGroups();
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

            private void LoadGroups()
            {
                NpgsqlCommand cmd = Connection.GetCommand("SELECT g.\"Id\", g.\"NumberGroup\", g.\"Lecturer\", u.\"FirstName\", u.\"LastName\", u.\"Patronymic\" FROM \"Group\" g, \"Lecturer\" l, \"User\" u " +
                    "WHERE g.\"Lecturer\" = l.\"Id\" AND l.\"Id\" = u.\"Id\"");
                NpgsqlDataReader result = cmd.ExecuteReader();

                while (result.Read())
                {
                    Groups.Add(new ClassGroup(
                                result.GetInt32(0),
                                result.GetString(1),
                                result.GetInt32(2),
                                result.GetString(3),
                                result.GetString(4),
                                result.GetString(5)
                        ));
                }

                result.Close();
            }

            private void LoadFilteredTimetable()
            {
                if (SelectedWeek != null)
                {
                    string query = "SELECT tl.\"Id\", tl.\"Time\", tl.\"Group\", g.\"NumberGroup\", tl.\"Office\", tl.\"Date\" " +
                                   "FROM \"TimetableTheory\" tl " +
                                   "JOIN \"Group\" g ON tl.\"Group\" = g.\"Id\" " +
                                   "WHERE tl.\"Date\" BETWEEN @StartDate AND @EndDate";

                    if (SelectedGroup != null)
                    {
                        query += " AND g.\"Id\" = @GroupName";
                    }

                    NpgsqlCommand command = Connection.GetCommand(query);
                    command.Parameters.AddWithValue("StartDate", SelectedWeek.StartDate);
                    command.Parameters.AddWithValue("EndDate", SelectedWeek.EndDate);

                    if (SelectedGroup != null)
                    {
                        command.Parameters.AddWithValue("GroupName", SelectedGroup.Id);
                    }

                    var reader = command.ExecuteReader();

                    ClearSchedules();

                    while (reader.Read())
                    {
                        var timetable = new TimeTable
                        {
                            Id = reader.GetInt32(0),
                            Time = reader.GetTimeSpan(1),
                            Group = reader.GetInt32(2),
                            NumberGroup = reader.GetString(3),
                            Office = reader.GetString(4),
                            Date = reader.GetDateTime(5),
                            Weekday = reader.GetDateTime(5).DayOfWeek.ToString()
                        };

                        switch (timetable.Weekday)
                        {
                            case "Monday":
                                MondaySchedule.Add(timetable);
                                break;
                            case "Tuesday":
                                TuesdaySchedule.Add(timetable);
                                break;
                            case "Wednesday":
                                WednesdaySchedule.Add(timetable);
                                break;
                            case "Thursday":
                                ThursdaySchedule.Add(timetable);
                                break;
                            case "Friday":
                                FridaySchedule.Add(timetable);
                                break;
                            case "Saturday":
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

            private void ClearSchedules()
            {
                MondaySchedule.Clear();
                TuesdaySchedule.Clear();
                WednesdaySchedule.Clear();
                ThursdaySchedule.Clear();
                FridaySchedule.Clear();
                SaturdaySchedule.Clear();
            }
            public void MarkCurrentWeek()
            {
                DateTime currentDate = DateTime.Today;
                var currentWeek = Weeks.FirstOrDefault(week => currentDate >= week.StartDate && currentDate <= week.EndDate);
                if (currentWeek != null)
                {
                    currentWeek.IsCurrentWeek = true;
                    SelectedWeek = currentWeek;
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
            NavigationService.Navigate(new AccountLector(_user));
        }

        private void btnTransitionSchedulePage(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Вы уже находитесь на данной странице");
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            cbGroup.Text = null;
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Connection.users = null;

            NavigationService.Navigate(new EntryPage());
        }
    }
}
