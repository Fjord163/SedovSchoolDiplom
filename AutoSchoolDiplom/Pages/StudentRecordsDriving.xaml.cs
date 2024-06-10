using AutoSchoolDiplom.ModalWindow;
using DBConnection;
using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Net;
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
    /// Логика взаимодействия для StudentRecordsDriving.xaml
    /// </summary>
    public partial class StudentRecordsDriving : Page
    {
        private CLassUser _currentUser;
        public StudentRecordsDriving(CLassUser authorizedStudent)
        {
            InitializeComponent();
            _currentUser = authorizedStudent;
            LoadDrivingRecords();
        }
        private void LoadDrivingRecords()
        {
            List<DrivingRecord> drivingRecords = new List<DrivingRecord>();

            try
            {
                NpgsqlCommand cmd = Connection.GetCommand("SELECT s.\"Date\", s.\"Time\", i.\"Id\" AS \"InstructorId\", u.\"FirstName\" || ' ' || u.\"LastName\" AS \"Instructor\" " +
                        "FROM \"Schedule\" s " +
                        "INNER JOIN \"Instructor\" i ON s.\"InstructorId\" = i.\"Id\" " +
                        "INNER JOIN \"User\" u ON i.\"Id\" = u.\"Id\" " +
                        "WHERE s.\"StudentId\" = @StudentId");
                cmd.Parameters.AddWithValue("@StudentId", _currentUser.Id);
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
                        InstructorId = result.GetInt32(2),
                        Instructor = result.GetString(3)
                    });
                }
                lvRecordsStudent.ItemsSource = drivingRecords;
                result.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке данных: " + ex.Message);
            }
        }

        private void CancelLesson_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            if (button.DataContext is DrivingRecord record)
            {
                ConfirmCancelDialog confirmDialog = new ConfirmCancelDialog(_currentUser, record);
                confirmDialog.Owner = Window.GetWindow(this);
                confirmDialog.ShowDialog();

                if (confirmDialog.IsConfirmed)
                {
                    string message = confirmDialog.Message;
                    try
                    {
                        NpgsqlCommand cmd = Connection.GetCommand("SELECT \"Id\" FROM \"Schedule\" WHERE \"Date\" = @Date AND \"Time\" = @Time AND \"InstructorId\" = @InstructorId ");
                        cmd.Parameters.AddWithValue("@Date", record.Date);
                        cmd.Parameters.AddWithValue("@Time", record.Time);
                        cmd.Parameters.AddWithValue("@InstructorId", record.InstructorId);

                        int scheduleId = (int)cmd.ExecuteScalar();
                        if (scheduleId != 0)
                        {
                            cmd = Connection.GetCommand("UPDATE \"Schedule\" SET \"IsBooked\" = false, \"StudentId\" = NULL WHERE \"Id\" = @ScheduleId");
                            cmd.Parameters.AddWithValue("@ScheduleId", scheduleId);
                            int rowsAffected = cmd.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Занятие успешно отменено. Сообщение отправлено.");
                                LoadDrivingRecords();
                            }
                            else
                            {
                                MessageBox.Show("Не удалось отменить занятие.");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Не удалось найти соответствующую запись в расписании.");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка при отмене занятия: " + ex.Message);
                    }
                }
            }
        }

        private void SendEmailToInstructor(int instructorId, string message)
        {
            try
            {
                NpgsqlCommand cmd = Connection.GetCommand("SELECT \"Email\" FROM \"User\" WHERE \"Id\" = @InstructorId");
                cmd.Parameters.AddWithValue("@InstructorId", instructorId);
                string instructorEmail = (string)cmd.ExecuteScalar();

                if (!string.IsNullOrEmpty(instructorEmail))
                {
                    MailMessage mail = new MailMessage();
                    mail.From = new MailAddress("clozed2003@mail.ru"); 
                    mail.To.Add(instructorEmail);
                    mail.Subject = "Отмена занятия";
                    mail.Body = message;

                    SmtpClient smtpClient = new SmtpClient("smtp.mail.ru")
                    {
                        Port = 587, // Замените на ваш порт
                        Credentials = new NetworkCredential("clozed2003@mail.ru", "9GzmiBHUTChEJ199VbkZ"), 
                        EnableSsl = true,
                    };

                    smtpClient.Send(mail);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при отправке сообщения на почту инструктора: " + ex.Message);
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
        private void btnTransmitionRecordDrivingPage(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RecordDriving(_currentUser));
        }
        private void btnTransitionSchedulePage(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ScheduleStudent(_currentUser));
        }
        private void btnTransitioAccountnPage(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AccountStudent(_currentUser));
        }
        private void btnTransmitionStudentRecordsDrivingPage(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Вы уже находитесь на данной странице");

        }
    }
}
