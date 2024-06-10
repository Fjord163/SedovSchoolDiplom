using DBConnection;
using Npgsql;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace AutoSchoolDiplom.ModalWindow
{
    /// <summary>
    /// Логика взаимодействия для ConfirmCancelDialog.xaml
    /// </summary>
    public partial class ConfirmCancelDialog : Window
    {
        private CLassUser _student;
        private DrivingRecord _record;
        private DateTime _date;
        public string Message { get; private set; }
        public bool IsConfirmed { get; private set; }

        public ConfirmCancelDialog(CLassUser student, DrivingRecord record)
        {
            InitializeComponent();
            IsConfirmed = false;
            _student = student;
            _record = record;
        }
        private void SendEmailToInstructor(int instructorId, string message)
        {
            try
            {
                NpgsqlCommand cmd = Connection.GetCommand("SELECT \"Email\", \"FirstName\", \"LastName\" FROM \"User\" WHERE \"Id\" = @InstructorId");
                cmd.Parameters.AddWithValue("@InstructorId", instructorId);
                NpgsqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    string instructorEmail = reader.GetString(0);
                    string studentName = _student.FirstName;
                    string studentLastName = _student.LastName;

                    if (!string.IsNullOrEmpty(instructorEmail))
                    {
                        MailMessage mail = new MailMessage();
                        mail.From = new MailAddress("clozed2003@mail.ru");
                        mail.To.Add(instructorEmail);
                        mail.Subject = "Отмена занятия";
                        mail.Body = $"Уважаемый {reader.GetString(1)} {reader.GetString(2)}, занятие с учеником {studentName} {studentLastName} на {_record.Date.ToShortDateString()} в {_record.Time} было отменено. Причина: {message}";

                        SmtpClient smtpClient = new SmtpClient("smtp.mail.ru")
                        {
                            Port = 587,
                            Credentials = new NetworkCredential("clozed2003@mail.ru", "9GzmiBHUTChEJ199VbkZ"),
                            EnableSsl = true,
                        };

                        smtpClient.Send(mail);
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при отправке сообщения на почту инструктора: " + ex.Message);
            }
        }
        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            Message = MessageTextBox.Text;
            IsConfirmed = true;
            SendEmailToInstructor(_record.InstructorId, Message);
            this.Close();
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            IsConfirmed = false;
            this.Close();
        }
    }
}
