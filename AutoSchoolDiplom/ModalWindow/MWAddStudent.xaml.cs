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
using static MaterialDesignThemes.Wpf.Theme;
using BCrypt.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Net;

namespace AutoSchoolDiplom.ModalWindow
{
    /// <summary>
    /// Логика взаимодействия для MWAddStudent.xaml
    /// </summary>
    public partial class MWAddStudent : Window
    {
        public MWAddStudent()
        {
            InitializeComponent();

            BindingcbGroup();
            BindingcbCours();
        }
        public void InsertStudentInfo(FullInfoStudent student)
        {
          
            if (string.IsNullOrWhiteSpace(tbLogin.Text) ||
            string.IsNullOrWhiteSpace(tbPass.Text) ||
            string.IsNullOrWhiteSpace(tbFirstName.Text) ||
            string.IsNullOrWhiteSpace(tbLastName.Text) ||
            string.IsNullOrWhiteSpace(tbPhone.Text) ||
            string.IsNullOrWhiteSpace(tbEmail.Text) ||
            dpDateBirth.SelectedDate == null ||
            cbCours.SelectedItem == null ||
            cbGroup.SelectedItem == null)
            {
                MessageBox.Show("Не все обязательные поля заполнены.");
                return;
            }

            CLassCours cours = cbCours.SelectedItem as CLassCours;
            ClassGroup group = cbGroup.SelectedItem as ClassGroup;
            var login = tbLogin.Text.Trim();
            var password = tbPass.Text.Trim();
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            var firstName = tbFirstName.Text.Trim();
            var lastName = tbLastName.Text.Trim();
            var patronymic = tbPatronymic.Text.Trim();
            var phone = tbPhone.Text.Trim();
            var email = tbEmail.Text.Trim();
            var birth = dpDateBirth.SelectedDate;
            string icon = "C:\\Users\\cloze\\source\\repos\\AutoSchoolDiplom\\AutoSchoolDiplom\\Image\\Icon.png";
            string role = "Студент";

            var currentYear = DateTime.Now.Year;
            var minimumBirthYear = currentYear - 16;

            if (birth.Value.Year > minimumBirthYear)
            {
                MessageBox.Show($"Ученик должен быть старше. Минимальный возраст 16. Проверьте данные.");
                return;
            }

            try
            {
                NpgsqlCommand cmd = Connection.GetCommand("insert into \"User\" (\"Login\", \"Password\", \"FirstName\", \"LastName\", \"Patronymic\", \"Phone\", \"Email\", \"DateBirth\", \"Role\")" +
                     "values (@login, @password, @firstName, @lastName, @patronymic, @phone, @email, @dateBirth, @role) returning \"Id\"");
                cmd.Parameters.AddWithValue("@login", NpgsqlDbType.Varchar, login);
                cmd.Parameters.AddWithValue("@password", NpgsqlDbType.Varchar, hashedPassword);
                cmd.Parameters.AddWithValue("@firstName", NpgsqlDbType.Varchar, firstName);
                cmd.Parameters.AddWithValue("@lastName", NpgsqlDbType.Varchar, lastName);
                cmd.Parameters.AddWithValue("@patronymic", NpgsqlDbType.Varchar, patronymic);
                cmd.Parameters.AddWithValue("@phone", NpgsqlDbType.Varchar, phone);
                cmd.Parameters.AddWithValue("@email", NpgsqlDbType.Varchar, email);
                cmd.Parameters.AddWithValue("@dateBirth", NpgsqlDbType.Date, birth);
                cmd.Parameters.AddWithValue("@role", NpgsqlDbType.Varchar, role);
                var result = cmd.ExecuteScalar();
                if (result != null)
                {
                    int IdStudent = student.Id = (int)result;

                    cmd = Connection.GetCommand("insert into \"Student\" (\"Id\", \"Photo\", \"Cours\") values (@id, @photo, @cours)");
                    cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, IdStudent);
                    cmd.Parameters.AddWithValue("@photo", NpgsqlDbType.Varchar, icon);
                    cmd.Parameters.AddWithValue("@cours", NpgsqlDbType.Integer, cours.Id);
                    result = cmd.ExecuteNonQuery();
                    if (result != null)
                    {
                        cmd = Connection.GetCommand("insert into \"StudentGroup\" (\"Student\", \"Group\") values (@student, @group)");
                        cmd.Parameters.AddWithValue("@student", NpgsqlDbType.Integer, IdStudent);
                        cmd.Parameters.AddWithValue("@group", NpgsqlDbType.Integer, group.Id);
                        result = cmd.ExecuteNonQuery();
                    }
                }
                if (result == null) { MessageBox.Show("Данные не добавлены"); }
                else { MessageBox.Show("Данные добавлены"); }
            }
            catch
            {
                MessageBox.Show("Заполните поля");
            }
        }
        public void BindingcbCours()
        {
            Binding binding = new Binding();
            binding.Source = Connection.cours;
            cbCours.SetBinding(ItemsControl.ItemsSourceProperty, binding);
            Connection.SelectCoursStudent();
        }
        public void BindingcbGroup()
        {
            Binding binding = new Binding();
            binding.Source = Connection.classGroups;
            cbGroup.SetBinding(ItemsControl.ItemsSourceProperty, binding);
            Connection.SelectGroup();
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MWCrateUser_Click(object sender, RoutedEventArgs e)
        {
            FullInfoStudent student = new FullInfoStudent();
            InsertStudentInfo(student);
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
            System.Windows.Controls.TextBox textBox = (System.Windows.Controls.TextBox)sender;
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
