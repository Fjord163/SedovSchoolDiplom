using DBConnection;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;

namespace AutoSchoolDiplom.ModalWindow
{

    public partial class MWTimetableAdd : Window
    {
        public ObservableCollection<TimeTable> MondaySchedule { get; set; }
        public ObservableCollection<TimeTable> TuesdaySchedule { get; set; }
        public ObservableCollection<TimeTable> WednesdaySchedule { get; set; }
        public ObservableCollection<TimeTable> ThursdaySchedule { get; set; }
        public ObservableCollection<TimeTable> FridaySchedule { get; set; }
        public ObservableCollection<TimeTable> SaturdaySchedule { get; set; }
        public ObservableCollection<TimeTable> SundaySchedule { get; set; }
        public MWTimetableAdd()
        {
            InitializeComponent();


            MondaySchedule = new ObservableCollection<TimeTable>();
            TuesdaySchedule = new ObservableCollection<TimeTable>();
            WednesdaySchedule = new ObservableCollection<TimeTable>();
            ThursdaySchedule = new ObservableCollection<TimeTable>();
            FridaySchedule = new ObservableCollection<TimeTable>();
            SaturdaySchedule = new ObservableCollection<TimeTable>();
            SundaySchedule = new ObservableCollection<TimeTable>();

            DataContext = this;
            LoadGroups();
            LoadOffice();
        }
        private void LoadGroups()
        {
            try
            {
                string query = "SELECT \"Id\", \"NumberGroup\" FROM \"Group\"";
                NpgsqlCommand command = Connection.GetCommand(query);
                NpgsqlDataReader reader = command.ExecuteReader();

                List<Group> groups = new List<Group>();
                while (reader.Read())
                {
                    Group group = new Group(
                        reader.GetInt32(0),
                        reader.GetString(1)
                    );
                    groups.Add(group);
                }
                reader.Close();

                cbGroups.ItemsSource = groups;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки групп: " + ex.Message);
            }
        }

        private void LoadOffice()
        {
            try
            {
                string query = "SELECT \"Name\" FROM \"Office\"";
                NpgsqlCommand command = Connection.GetCommand(query);
                NpgsqlDataReader reader = command.ExecuteReader();

                List<Office> offices = new List<Office>();
                while (reader.Read())
                {
                    Office office = new Office
                    {
                        Name = reader.GetString(0)
                    };
                    offices.Add(office);
                }
                reader.Close();

                cbOffice.ItemsSource = offices;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки кабинетов: " + ex.Message);
            }
        }
        private enum ScheduleConflictType
        {
            None,
            TimeOccupied,
            GroupHasSchedule
        }

        private ScheduleConflictType CanAddNewSchedule(int groupId, DateTime newDate)
        {
            try
            {
                string timeOccupiedQuery = "SELECT COUNT(*) FROM \"TimetableTheory\" " +
                                           "WHERE \"Date\" = @Date " +
                                           "AND EXTRACT(HOUR FROM \"Time\") = @Hour " +
                                           "AND EXTRACT(MINUTE FROM \"Time\") BETWEEN @Minute AND @Minute + 59";
                NpgsqlCommand timeOccupiedCommand = Connection.GetCommand(timeOccupiedQuery);
                timeOccupiedCommand.Parameters.AddWithValue("Date", newDate.Date);
                timeOccupiedCommand.Parameters.AddWithValue("Hour", newDate.Hour);
                timeOccupiedCommand.Parameters.AddWithValue("Minute", newDate.Minute);

                int timeOccupiedCount = Convert.ToInt32(timeOccupiedCommand.ExecuteScalar());
                if (timeOccupiedCount > 0)
                {
                    return ScheduleConflictType.TimeOccupied;
                }

                string groupScheduleQuery = "SELECT COUNT(*) FROM \"TimetableTheory\" " +
                                            "WHERE \"Group\" = @GroupId " +
                                            "AND \"Date\" = @Date";
                NpgsqlCommand groupScheduleCommand = Connection.GetCommand(groupScheduleQuery);
                groupScheduleCommand.Parameters.AddWithValue("GroupId", groupId);
                groupScheduleCommand.Parameters.AddWithValue("Date", newDate.Date);

                int groupScheduleCount = Convert.ToInt32(groupScheduleCommand.ExecuteScalar());
                if (groupScheduleCount > 0)
                {
                    return ScheduleConflictType.GroupHasSchedule;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при проверке возможности добавления расписания: {ex.Message}");
                return ScheduleConflictType.None;
            }

            return ScheduleConflictType.None;
        }

        private void GenerateScheduleButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cbGroups.SelectedItem == null)
                {
                    MessageBox.Show("Выберите группу для создания расписания.");
                    return;
                }
                if (cbOffice.SelectedItem == null)
                {
                    MessageBox.Show("Выберите кабинет для создания расписания.");
                    return;
                }

                ClearSchedules();

                int groupId = (int)cbGroups.SelectedValue;
                string office = cbOffice.SelectedValue.ToString();
                List<int> selectedDaysOfWeek = GetSelectedDaysOfWeek();
                TimeSpan startTime = TimeSpan.Parse((cbStartTime.SelectedItem as ComboBoxItem).Content.ToString());

                List<DateTime> datesToAdd = new List<DateTime>();
                DateTime currentDate = DateTime.Now.Date;

                // Определяем дату следующего понедельника
                int daysUntilNextMonday = ((int)DayOfWeek.Monday - (int)currentDate.DayOfWeek + 7) % 7;
                DateTime nextMonday = currentDate.AddDays(daysUntilNextMonday);

                foreach (var dayOfWeek in selectedDaysOfWeek)
                {
                    DateTime targetDate = nextMonday.AddDays(dayOfWeek - 1); // -1 потому что Monday = 1
                    DateTime dateToAdd = targetDate.Add(startTime);
                    datesToAdd.Add(dateToAdd);
                }

                foreach (var date in datesToAdd)
                {
                    var conflictType = CanAddNewSchedule(groupId, date);
                    if (conflictType == ScheduleConflictType.TimeOccupied)
                    {
                        MessageBox.Show($"Нельзя добавить занятие в {date.ToShortDateString()} {date.ToShortTimeString()}. Это время занято.");
                        return;
                    }
                    else if (conflictType == ScheduleConflictType.GroupHasSchedule)
                    {
                        MessageBox.Show($"У данной группы уже есть занятие на {date.ToShortDateString()}.");
                        return;
                    }
                }

                MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите сформировать расписание?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.No)
                {
                    return;
                }

                foreach (var date in datesToAdd)
                {
                    string insertQuery = "INSERT INTO \"TimetableTheory\" (\"Time\", \"Group\", \"Office\", \"Date\") " +
                                         "VALUES (@Time, @Group, @Office, @Date)";

                    NpgsqlCommand insertCommand = Connection.GetCommand(insertQuery);
                    insertCommand.Parameters.AddWithValue("Time", date.TimeOfDay);
                    insertCommand.Parameters.AddWithValue("Group", groupId);
                    insertCommand.Parameters.AddWithValue("Office", office);
                    insertCommand.Parameters.AddWithValue("Date", date);

                    insertCommand.ExecuteNonQuery();
                }

                lbDaysOfWeek.SelectedItems.Clear();
                MessageBox.Show("Расписание успешно сформировано и добавлено в базу данных.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при генерации и добавлении расписания: {ex.Message}");
            }
            //try
            //{
            //    if (cbGroups.SelectedItem == null)
            //    {
            //        MessageBox.Show("Выберите группу для создания расписания.");
            //        return;
            //    }
            //    if (cbOffice.SelectedItem == null)
            //    {
            //        MessageBox.Show("Выберите кабинет для создания расписания.");
            //        return;
            //    }

            //    ClearSchedules();

            //    int groupId = (int)cbGroups.SelectedValue;
            //    string office = cbOffice.SelectedValue.ToString();
            //    List<int> selectedDaysOfWeek = GetSelectedDaysOfWeek();
            //    TimeSpan startTime = TimeSpan.Parse((cbStartTime.SelectedItem as ComboBoxItem).Content.ToString());

            //    List<DateTime> datesToAdd = new List<DateTime>();
            //    DateTime currentDate = DateTime.Now.Date;

            //    foreach (var dayOfWeek in selectedDaysOfWeek)
            //    {
            //        DateTime targetDate = currentDate.AddDays((dayOfWeek - (int)currentDate.DayOfWeek + 7) % 7);
            //        DateTime dateToAdd = targetDate.Add(startTime);
            //        datesToAdd.Add(dateToAdd);
            //    }

            //    foreach (var date in datesToAdd)
            //    {
            //        var conflictType = CanAddNewSchedule(groupId, date);
            //        if (conflictType == ScheduleConflictType.TimeOccupied)
            //        {
            //            MessageBox.Show($"Нельзя добавить занятие в {date.ToShortDateString()} {date.ToShortTimeString()}. Это время занято.");
            //            return;
            //        }
            //        else if (conflictType == ScheduleConflictType.GroupHasSchedule)
            //        {
            //            MessageBox.Show($"У данной группы уже есть занятие на {date.ToShortDateString()}.");
            //            return;
            //        }
            //    }

            //    MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите сформировать расписание?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            //    if (result == MessageBoxResult.No)
            //    {
            //        return;
            //    }

            //    foreach (var date in datesToAdd)
            //    {
            //        string insertQuery = "INSERT INTO \"TimetableTheory\" (\"Time\", \"Group\", \"Office\", \"Date\") " +
            //                             "VALUES (@Time, @Group, @Office, @Date)";

            //        NpgsqlCommand insertCommand = Connection.GetCommand(insertQuery);
            //        insertCommand.Parameters.AddWithValue("Time", date.TimeOfDay);
            //        insertCommand.Parameters.AddWithValue("Group", groupId);
            //        insertCommand.Parameters.AddWithValue("Office", office);
            //        insertCommand.Parameters.AddWithValue("Date", date);

            //        insertCommand.ExecuteNonQuery();
            //    }

            //    lbDaysOfWeek.SelectedItems.Clear();
            //    MessageBox.Show("Расписание успешно сформировано и добавлено в базу данных.");
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show($"Произошла ошибка при генерации и добавлении расписания: {ex.Message}");
            //}
        }

        private List<int> GetSelectedDaysOfWeek()
        {
            List<int> daysOfWeek = new List<int>();
            foreach (ListBoxItem item in lbDaysOfWeek.SelectedItems)
            {
                switch (item.Content.ToString())
                {
                    case "Понедельник":
                        daysOfWeek.Add(1);
                        break;
                    case "Вторник":
                        daysOfWeek.Add(2);
                        break;
                    case "Среда":
                        daysOfWeek.Add(3);
                        break;
                    case "Четверг":
                        daysOfWeek.Add(4);
                        break;
                    case "Пятница":
                        daysOfWeek.Add(5);
                        break;
                    case "Суббота":
                        daysOfWeek.Add(6);
                        break;
                    case "Воскресенье":
                        daysOfWeek.Add(0);
                        break;
                }
            }
            return daysOfWeek;
        }


        private void ClearSchedules()
        {
            MondaySchedule.Clear();
            TuesdaySchedule.Clear();
            WednesdaySchedule.Clear();
            ThursdaySchedule.Clear();
            FridaySchedule.Clear();
            SaturdaySchedule.Clear();
            SundaySchedule.Clear();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
    }
}
