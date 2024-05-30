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
            private string _timeSlotText;
            private ClassNameWeek _selectedWeek;
            private ClassGroup _selectedGroup;


            public ObservableCollection<ClassNameWeek> Weeks { get; set; }
            public ObservableCollection<TimeTable> Timetable { get; set; }
            public ObservableCollection<ClassGroup> Groups { get; set; }
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
                    LoadTimeSlots();
                    LoadFilteredTimetable();
                }
            }

            public string TimeSlotText
            {
                get { return _timeSlotText; }
                set
                {
                    _timeSlotText = value;
                    OnPropertyChanged();
                }
            }

            public ScheduleViewModel()
            {
                Weeks = new ObservableCollection<ClassNameWeek>();
                Groups = new ObservableCollection<ClassGroup>();
                Timetable = new ObservableCollection<TimeTable>();
                LoadWeeks();
                LoadGroups(); 
            }

            private void LoadWeeks()
            {
                NpgsqlCommand cmd = Connection.GetCommand("SELECT \"Id\", \"NumberWeek\", \"StartWeek\", \"EndWeek\" FROM \"NameWeek\"");
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
                    string query = "SELECT tl.\"Id\", tl.\"NumberWeek\", tl.\"Weekday\", tl.\"Time\", tl.\"Group\", g.\"NumberGroup\"" +
                                "FROM \"TimetableTheory\" tl " +
                                "JOIN \"Group\" g ON tl.\"Group\" = g.\"Id\"" +
                                "WHERE tl.\"NumberWeek\" = @NumberWeek";
                    

                    NpgsqlCommand command = Connection.GetCommand(query);
                    command.Parameters.AddWithValue("NumberWeek", SelectedWeek.Id);
                    if (SelectedGroup != null)
                    {
                        query += " AND g.\"Id\" = @GroupName";
                        command.CommandText = query;
                        command.Parameters.AddWithValue("GroupName", SelectedGroup?.Id);

                    }
                    var reader = command.ExecuteReader();
                    Timetable.Clear();
                    while (reader.Read())
                    {
                        Timetable.Add(new TimeTable
                        {
                            Id = reader.GetInt32(0),
                            NumberWeek = reader.GetInt32(1),
                            Weekday = reader.GetString(2),
                            Time = reader.GetTimeSpan(3),
                            Group = reader.GetInt32(4),
                            NumberGroup = reader.GetString(5)
                        });
                    }
                    reader.Close();
                }
                else
                {
                    Timetable.Clear();
                }
            }

            private void LoadTimeSlots()
            {
                if (SelectedWeek != null)
                {
                    TimeSlotText = $"{SelectedWeek.StartWeek.ToString("d")} - {SelectedWeek.EndWeek.ToString("d")}";
                }
                else
                {
                    TimeSlotText = string.Empty;
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
    }
}
