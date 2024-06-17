using DBConnection;
using Npgsql;
using NpgsqlTypes;
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
using System.Windows.Shapes;

namespace AutoSchoolDiplom.ModalWindow
{
    /// <summary>
    /// Логика взаимодействия для MWEditLecturer.xaml
    /// </summary>
    public partial class MWEditLecturer : Window
    {
        private FullInfoLecturer _fullinfoLecturer;
        public MWEditLecturer(FullInfoLecturer fullInfoLecturer)
        {
            InitializeComponent();

            _fullinfoLecturer = fullInfoLecturer;
            DataContext = fullInfoLecturer;
        }

        private void MWUpdateUser_Click(object sender, RoutedEventArgs e)
        {
           

            if (string.IsNullOrWhiteSpace(tbLogin.Text) ||
            string.IsNullOrWhiteSpace(tbPass.Text) ||
            string.IsNullOrWhiteSpace(tbFirstName.Text) ||
            string.IsNullOrWhiteSpace(tbLastName.Text) ||
            string.IsNullOrWhiteSpace(tbPhone.Text) ||
            string.IsNullOrWhiteSpace(tbEmail.Text) ||
            dpDateBirth.SelectedDate == null ||
            dpDateEmployment.SelectedDate == null)
            {
                MessageBox.Show("Не все обязательные поля заполнены.");
                return;
            }
            var password = tbPass.Text.Trim();
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            var birth = dpDateBirth.SelectedDate;
            var dateEmployment = dpDateEmployment.SelectedDate;


            var currentYear = DateTime.Now.Year;
            var minimumBirthYear = currentYear - 21;

            if (birth.Value.Year > minimumBirthYear)
            {
                MessageBox.Show($"Инструктор должен быть старше 21 года. Проверьте данные.");
                return;
            }
            try
            {
                NpgsqlCommand cmd = Connection.GetCommand("UPDATE \"Lecturer\" SET \"DateEmployment\"= @dateEmployment where \"Id\" = @id");
                cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, _fullinfoLecturer.Id);
                cmd.Parameters.AddWithValue("@dateEmployment", NpgsqlDbType.Date, dateEmployment);
                var result = cmd.ExecuteNonQuery();
                if (result != 0)
                {
                    cmd = Connection.GetCommand("UPDATE \"User\" SET \"Login\"= @login, \"Password\"= @password, \"FirstName\"= @firstName, \"LastName\"= @lastName," +
                        "\"Patronymic\"= @patronymic, \"Phone\"= @phone, \"Email\"= @email, \"DateBirth\"= @birth where \"Id\" = @id");
                    cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, _fullinfoLecturer.Id);
                    cmd.Parameters.AddWithValue("@login", NpgsqlDbType.Varchar, _fullinfoLecturer.Login);
                    cmd.Parameters.AddWithValue("@password", NpgsqlDbType.Varchar, hashedPassword);
                    cmd.Parameters.AddWithValue("@firstName", NpgsqlDbType.Varchar, _fullinfoLecturer.FirstName);
                    cmd.Parameters.AddWithValue("@lastName", NpgsqlDbType.Varchar, _fullinfoLecturer.LastName);
                    cmd.Parameters.AddWithValue("@patronymic", NpgsqlDbType.Varchar, _fullinfoLecturer.Patronymic);
                    cmd.Parameters.AddWithValue("@phone", NpgsqlDbType.Varchar, _fullinfoLecturer.Phone);
                    cmd.Parameters.AddWithValue("@email", NpgsqlDbType.Varchar, _fullinfoLecturer.Email);
                    cmd.Parameters.AddWithValue("@birth", NpgsqlDbType.Date, birth);
                    result = cmd.ExecuteNonQuery();
                }
                if (result != 0) { MessageBox.Show("Данные обновлены"); }
                else { MessageBox.Show("Данные не обновлены"); }
            }
            catch
            {
                MessageBox.Show("Заполните поля");
            }
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

        private void tbPhone_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1) || tbPhone.Text.Length >= 11)
            {
                e.Handled = true;
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string text = textBox.Text.Trim();

            if (string.IsNullOrEmpty(text))
                return;

            if (!IsAllLetters(text))
            {
                MessageBox.Show("Поле должно содержать только буквы.");
                textBox.Text = "";
                return;
            }

            textBox.Text = CorrectCase(text);
            textBox.SelectionStart = textBox.Text.Length;
        }

        private bool IsAllLetters(string text)
        {
            foreach (char c in text)
            {
                if (!char.IsLetter(c))
                    return false;
            }
            return true;
        }

        private string CorrectCase(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;
            return char.ToUpper(text[0]) + text.Substring(1).ToLower();
        }
    }
}
