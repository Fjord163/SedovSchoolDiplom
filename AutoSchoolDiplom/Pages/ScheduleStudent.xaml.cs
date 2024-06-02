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
            private string _timeSlotText;
            private ClassNameWeek _selectedWeek;
            private CLassUser _authorizedStudent;

            public ObservableCollection<ClassNameWeek> Weeks { get; set; }
            public ObservableCollection<TimeTable> MondaySchedule { get; set; }
            public ObservableCollection<TimeTable> TuesdaySchedule { get; set; }
            public ObservableCollection<TimeTable> WednesdaySchedule { get; set; }
            public ObservableCollection<TimeTable> ThursdaySchedule { get; set; }
            public ObservableCollection<TimeTable> FridaySchedule { get; set; }
            public ObservableCollection<TimeTable> SaturdaySchedule { get; set; }

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
                }
            }

            private void LoadWeeks()
            {
                try
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
                catch
                {
                    MessageBox.Show("Ошибка загрузки недель: ");
                }
            }

            private void LoadFilteredTimetable()
            {
                try
                {
                    if (SelectedWeek != null && AuthorizedStudent != null)
                    {
                        string query = "SELECT tl.\"Id\", tl.\"NumberWeek\", tl.\"Weekday\", tl.\"Time\", tl.\"Group\", g.\"NumberGroup\" " +
                                       "FROM \"TimetableTheory\" tl " +
                                       "JOIN \"Group\" g ON tl.\"Group\" = g.\"Id\" " +
                                       "JOIN \"StudentGroup\" sg ON g.\"Id\" = sg.\"Group\" " +
                                       "WHERE tl.\"NumberWeek\" = @NumberWeek::integer AND sg.\"Student\" = @StudentId::integer";

                        NpgsqlCommand command = Connection.GetCommand(query);
                        command.Parameters.AddWithValue("NumberWeek", SelectedWeek.Id);
                        command.Parameters.AddWithValue("StudentId", AuthorizedStudent.Id);

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
                                NumberGroup = reader.GetString(5)
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
                catch
                {
                    MessageBox.Show("Ошибка загрузки расписания: ");
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

        //public class ScheduleViewModel : INotifyPropertyChanged
        //{
        //    private string _timeSlotText;
        //    private ClassNameWeek _selectedWeek;
        //    private CLassUser _authorizedStudent;

        //    public ObservableCollection<ClassNameWeek> Weeks { get; set; }
        //    public ObservableCollection<TimeTable> Timetable { get; set; }

        //    public ClassNameWeek SelectedWeek
        //    {
        //        get { return _selectedWeek; }
        //        set
        //        {
        //            _selectedWeek = value;
        //            OnPropertyChanged();
        //            LoadTimeSlots();
        //            LoadFilteredTimetable();
        //        }
        //    }

        //    public string TimeSlotText
        //    {
        //        get { return _timeSlotText; }
        //        set
        //        {
        //            _timeSlotText = value;
        //            OnPropertyChanged();
        //        }
        //    }

        //    public ScheduleViewModel()
        //    {
        //        Weeks = new ObservableCollection<ClassNameWeek>();
        //        Timetable = new ObservableCollection<TimeTable>();
        //        LoadWeeks();
        //    }
        //    public CLassUser AuthorizedStudent // Новое свойство
        //    {
        //        get { return _authorizedStudent; }
        //        set
        //        {
        //            _authorizedStudent = value;
        //            OnPropertyChanged();
        //            LoadFilteredTimetable();
        //        }
        //    }

        //    private void LoadWeeks()
        //    {
        //        try
        //        {
        //            NpgsqlCommand cmd = Connection.GetCommand("SELECT \"Id\", \"NumberWeek\", \"StartWeek\", \"EndWeek\" FROM \"NameWeek\"");
        //            NpgsqlDataReader result = cmd.ExecuteReader();

        //            while (result.Read())
        //            {
        //                Weeks.Add(new ClassNameWeek(
        //                            result.GetInt32(0),
        //                            result.GetString(1),
        //                            result.GetDateTime(2),
        //                            result.GetDateTime(3)
        //                    ));
        //            }

        //            result.Close();
        //        }
        //        catch
        //        {
        //            MessageBox.Show("Ошибка загрузки недель: ");
        //        }
        //    }
        //    private void LoadFilteredTimetable()
        //    {
        //        try
        //        {
        //            if (SelectedWeek != null && AuthorizedStudent != null)
        //            {
        //                string query = "SELECT tl.\"Id\", tl.\"NumberWeek\", tl.\"Weekday\", tl.\"Time\", tl.\"Group\", g.\"NumberGroup\" " +
        //                               "FROM \"TimetableTheory\" tl " +
        //                               "JOIN \"Group\" g ON tl.\"Group\" = g.\"Id\" " +
        //                               "JOIN \"StudentGroup\" sg ON g.\"Id\" = sg.\"Group\" " +
        //                               "WHERE tl.\"NumberWeek\" = @NumberWeek::integer AND sg.\"Student\" = @StudentId::integer";

        //                NpgsqlCommand command = Connection.GetCommand(query);
        //                command.Parameters.AddWithValue("NumberWeek", SelectedWeek.Id);
        //                command.Parameters.AddWithValue("StudentId", AuthorizedStudent.Id);

        //                var reader = command.ExecuteReader();
        //                Timetable.Clear();
        //                while (reader.Read())
        //                {
        //                    Timetable.Add(new TimeTable
        //                    {
        //                        Id = reader.GetInt32(0),
        //                        NumberWeek = reader.GetInt32(1),
        //                        Weekday = reader.GetString(2),
        //                        Time = reader.GetTimeSpan(3),
        //                        Group = reader.GetInt32(4),
        //                        NumberGroup = reader.GetString(5)
        //                    });
        //                }
        //                reader.Close();
        //            }
        //            else
        //            {
        //                Timetable.Clear();
        //            }
        //        }
        //        catch
        //        {
        //            MessageBox.Show("Ошибка загрузки расписания: ");
        //        }
        //    }

        //    private void LoadTimeSlots()
        //    {
        //        if (SelectedWeek != null)
        //        {
        //            TimeSlotText = $"{SelectedWeek.StartWeek.ToString("d")} - {SelectedWeek.EndWeek.ToString("d")}";
        //        }
        //        else
        //        {
        //            TimeSlotText = string.Empty;
        //        }
        //    }

        //    public event PropertyChangedEventHandler PropertyChanged;

        //    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        //    {
        //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //    }
        //}

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
    }
}
