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
                NpgsqlCommand cmd = Connection.GetCommand("SELECT \"Id\", \"NumberWeek\", \"StartWeek\", \"EndWeek\" FROM \"NameWeek\" ORDER BY \"Id\" ASC");
                NpgsqlDataReader result = cmd.ExecuteReader();

                while (result.Read())
                {
                    Weeks.Add(new ClassNameWeek(
                                result.GetInt32(0),
                                result.GetString(1),
                                result.GetDateTime(2),
                                result.GetDateTime(3)
                        ));
                }

                result.Close();
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
                    string query = "SELECT tl.\"Id\", tl.\"NumberWeek\", tl.\"Weekday\", tl.\"Time\", tl.\"Group\", g.\"NumberGroup\", tl.\"Office\" " +
                                   "FROM \"TimetableTheory\" tl " +
                                   "JOIN \"Group\" g ON tl.\"Group\" = g.\"Id\" " +
                                   "WHERE tl.\"NumberWeek\" = @NumberWeek";

                    if (SelectedGroup != null)
                    {
                        query += " AND g.\"Id\" = @GroupName";
                    }

                    NpgsqlCommand command = Connection.GetCommand(query);
                    command.Parameters.AddWithValue("NumberWeek", SelectedWeek.Id);

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
                            NumberWeek = reader.GetInt32(1),
                            Weekday = reader.GetString(2),
                            Time = reader.GetTimeSpan(3),
                            Group = reader.GetInt32(4),
                            NumberGroup = reader.GetString(5),
                            Office = reader.GetString(6)
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

            private void ClearSchedules()
            {
                MondaySchedule.Clear();
                TuesdaySchedule.Clear();
                WednesdaySchedule.Clear();
                ThursdaySchedule.Clear();
                FridaySchedule.Clear();
                SaturdaySchedule.Clear();
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
    }
}
