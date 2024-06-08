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
    /// Логика взаимодействия для RecordDriving.xaml
    /// </summary>
    public partial class RecordDriving : Page
    {
        private List<SheduleItem> scheduleItems;
        private int StudentInstructorId;
        private CLassUser _currentUser;

        private string connectionString = "Host=localhost;Port=5432;Username=postgres;Password=1234;Database=SchoolSedov";
        public RecordDriving(CLassUser authorizedStudent)
        {
            InitializeComponent();
            _currentUser = authorizedStudent;
            LoadStudentInstructor();
            EnsureScheduleData();
            LoadSchedule();
            PopulateCalendarGrid();
        }
        private void EnsureScheduleData()
        {
            DateTime startDate = DateTime.Now.Date.AddDays(1); // Получаем завтрашнюю дату
            int daysToShow = 6; // Количество дней для отображения
            DateTime endDateToDelete = startDate.AddDays(-1);

            List<DateTime> dates = new List<DateTime>();
            for (int i = 0; i <= daysToShow; i++)
            {
                dates.Add(startDate.AddDays(i));
            }

            List<string> times = new List<string>();
            for (int i = 0; i < 8; i++) // 8 временных слотов с 10:00 до 17:00
            {
                times.Add($"{10 + i}:00");
            }

            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();

                // Удаляем старые записи
                string deleteSql = "DELETE FROM \"Schedule\" WHERE \"Date\" < @EndDateToDelete";
                using (NpgsqlCommand deleteCommand = new NpgsqlCommand(deleteSql, conn))
                {
                    deleteCommand.Parameters.AddWithValue("@EndDateToDelete", endDateToDelete);
                    deleteCommand.ExecuteNonQuery();
                }

                foreach (var date in dates)
                {
                    foreach (var time in times)
                    {
                        // Проверяем, существует ли запись
                        string checkSql = "SELECT COUNT(*) FROM \"Schedule\" WHERE \"Date\" = @Date AND \"Time\" = @Time AND \"InstructorId\" = @InstructorId";
                        using (NpgsqlCommand checkCommand = new NpgsqlCommand(checkSql, conn))
                        {
                            checkCommand.Parameters.AddWithValue("@Date", date);
                            checkCommand.Parameters.AddWithValue("@Time", time);
                            checkCommand.Parameters.AddWithValue("@InstructorId", StudentInstructorId);
                            long count = (long)checkCommand.ExecuteScalar();
                            if (count == 0)
                            {
                                // Проверяем, существует ли такой InstructorId
                                string checkInstructorSql = "SELECT COUNT(*) FROM \"Instructor\" WHERE \"Id\" = @InstructorId";
                                using (NpgsqlCommand checkInstructorCommand = new NpgsqlCommand(checkInstructorSql, conn))
                                {
                                    checkInstructorCommand.Parameters.AddWithValue("@InstructorId", StudentInstructorId);
                                    long instructorCount = (long)checkInstructorCommand.ExecuteScalar();
                                    if (instructorCount > 0)
                                    {
                                        // Вставляем новую запись
                                        string insertSql = "INSERT INTO \"Schedule\" (\"Date\", \"Time\", \"IsBooked\", \"StudentId\", \"InstructorId\") VALUES (@Date, @Time, @IsBooked, @StudentId, @InstructorId)";
                                        using (NpgsqlCommand insertCommand = new NpgsqlCommand(insertSql, conn))
                                        {
                                            insertCommand.Parameters.AddWithValue("@Date", date);
                                            insertCommand.Parameters.AddWithValue("@Time", time);
                                            insertCommand.Parameters.AddWithValue("@IsBooked", false);
                                            insertCommand.Parameters.AddWithValue("@StudentId", DBNull.Value);
                                            insertCommand.Parameters.AddWithValue("@InstructorId", StudentInstructorId);
                                            insertCommand.ExecuteNonQuery();
                                        }
                                    }
                                    else
                                    {
                                        // Инструктор не найден, обработка ошибки или логгирование
                                        MessageBox.Show($"Инструктор с ID {StudentInstructorId} не найден.");
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        private void LoadStudentInstructor()
        {
            try
            {
                NpgsqlCommand cmd = Connection.GetCommand("SELECT i.* FROM \"Instructor\" i INNER JOIN \"StudentInstructor\" si ON i.\"Id\" = si.\"Instructor\" WHERE si.\"Student\" = @StudentId");
                cmd.Parameters.AddWithValue("StudentId", _currentUser.Id);
                NpgsqlDataReader result = cmd.ExecuteReader();

                if (result.HasRows)
                {
                    result.Read();
                    int instructorName = result.GetInt32(result.GetOrdinal("Id"));
                    StudentInstructorId = instructorName;
                }
                result.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки инструктора студента: " + ex.Message);
            }
        }

        private void LoadSchedule()
        {
            scheduleItems = new List<SheduleItem>();

            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = "SELECT * FROM \"Schedule\" WHERE \"InstructorId\" = @InstructorId";
                    using (NpgsqlCommand command = new NpgsqlCommand(sql, conn))
                    {
                        command.Parameters.AddWithValue("@InstructorId", StudentInstructorId);
                        using (NpgsqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                scheduleItems.Add(new SheduleItem
                                {
                                    Id = reader.GetInt32(0),
                                    Date = reader.GetDateTime(1),
                                    Time = reader.GetString(2),
                                    IsBooked = reader.GetBoolean(3)
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке данных: " + ex.Message);
            }
        }
        private void PopulateCalendarGrid()
        {
            DateTime startDate = DateTime.Now.Date.AddDays(1);
            int daysToShow = 6;

            CalendarGrid.Children.Clear();
            CalendarGrid.ColumnDefinitions.Clear();
            CalendarGrid.RowDefinitions.Clear();

            for (int i = 0; i <= daysToShow; i++)
            {
                CalendarGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            }

            for (int i = 0; i < 8; i++)
            {
                CalendarGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            }

            for (int i = 0; i <= daysToShow; i++)
            {
                DateTime date = startDate.AddDays(i);
                bool isWeekend = date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;

                TextBlock dayHeader = new TextBlock
                {
                    Text = date.ToString("ddd dd"),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontWeight = FontWeights.Bold
                };

                if (isWeekend)
                {
                    dayHeader.TextDecorations = TextDecorations.Strikethrough;
                }

                Grid.SetRow(dayHeader, 0);
                Grid.SetColumn(dayHeader, i);
                CalendarGrid.Children.Add(dayHeader);

                for (int j = 0; j < 8; j++)
                {
                    string time = $"{10 + j}:00";
                    SheduleItem item = scheduleItems.FirstOrDefault(si => si.Date == date && si.Time == time);

                    Button timeButton = new Button
                    {
                        Content = time,
                        Background = item != null && item.IsBooked ? Brushes.Orange : Brushes.Green,
                        Tag = item
                    };

                    if (isWeekend)
                    {
                        timeButton.IsEnabled = false;
                        timeButton.ToolTip = "Этот временной слот недоступен в выходные";
                    }
                    else
                    {
                        timeButton.Click += TimeButton_Click;
                    }

                    Grid.SetRow(timeButton, j + 1);
                    Grid.SetColumn(timeButton, i);
                    CalendarGrid.Children.Add(timeButton);
                }
            }
        }
        private void TimeButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                SheduleItem item = button.Tag as SheduleItem;
                if (item != null)
                {
                    if (!item.IsBooked)
                    {
                        MessageBoxResult result = MessageBox.Show($"Вы хотите записаться на занятие {item.Date:dd.MM.yyyy} в {item.Time}?", "Запись на занятие", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (result == MessageBoxResult.Yes)
                        {
                            int studentId = _currentUser.Id;
                            int instructorId = StudentInstructorId;
                            if (studentId != 0 && instructorId != 0)
                            {
                                item.IsBooked = true;
                                bool success = UpdateScheduleItem(item, studentId, instructorId);
                                if (success)
                                {
                                    button.Background = Brushes.Orange;
                                    MessageBox.Show($"Вы успешно записались на занятие: {item.Date:dd.MM.yyyy} в {item.Time}");
                                }
                                else
                                {
                                    MessageBox.Show("Ошибка при записи на занятие.");
                                }
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Этот временной слот уже забронирован.");
                    }
                }
                else
                {
                    MessageBox.Show("Не удалось найти информацию о выбранном временном слоте.");
                }
            }
        }

        private bool UpdateScheduleItem(SheduleItem item, int studentId, int instructorId)
        {
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = "UPDATE \"Schedule\" SET \"IsBooked\" = @IsBooked, \"StudentId\" = @StudentId, \"InstructorId\" = @InstructorId WHERE \"Id\" = @Id";
                    using (NpgsqlCommand command = new NpgsqlCommand(sql, conn))
                    {
                        command.Parameters.AddWithValue("@IsBooked", item.IsBooked);
                        command.Parameters.AddWithValue("@StudentId", studentId);
                        command.Parameters.AddWithValue("@InstructorId", instructorId);
                        command.Parameters.AddWithValue("@Id", item.Id);
                        command.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при обновлении данных: " + ex.Message);
                return false;
            }
        }

        private void btnTransitionSchedulePage(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ScheduleStudent(_currentUser));
        }
        private void btnTransmitionAccountPage(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AccountStudent(_currentUser));
        }
        private void btnTransmitionRecordDrivingPage(object sender, RoutedEventArgs e)
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
    }
}
