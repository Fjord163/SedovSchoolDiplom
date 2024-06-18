using AutoSchoolDiplom.Pages;
using DBConnection;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace AutoSchoolDiplom.ModalWindow
{
    /// <summary>
    /// Логика взаимодействия для MWEditStudent.xaml
    /// </summary>
    public partial class MWEditStudent : Window
    {
        private FullInfoStudent _fullinfoStudent;
        private FullInfoStudent _student;
        public ObservableCollection<string> Programs { get; set; }
        public ObservableCollection<Instructor> Instructors { get; set; }
        public MWEditStudent(FullInfoStudent fullInfoStudent, List<StudentComboBoxItem> comboBoxItems, StudentComboBoxItem selectedComboBoxItem)
        {
            InitializeComponent();

            _fullinfoStudent = fullInfoStudent;
            DataContext = fullInfoStudent;
            Instructors = new ObservableCollection<Instructor>();

            InitializeUI();
        }

        private void InitializeUI()
        {
            LoadInstructors();
            LoadStudentInstructor();
            BindingcbGroup();
        }
        private void LoadInstructors()
        {
            try
            {
                Instructors.Clear();

                NpgsqlCommand cmd = Connection.GetCommand("SELECT i.\"Id\", u.\"FirstName\", u.\"LastName\", u.\"Patronymic\" FROM \"Instructor\" i JOIN \"User\" u ON i.\"Id\" = u.\"Id\"");
                NpgsqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Instructors.Add(new Instructor
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                        LastName = reader.GetString(reader.GetOrdinal("LastName")),
                        Patronymic = reader.GetString(reader.GetOrdinal("Patronymic"))
                    });
                }
                reader.Close();
                cbInstructor.ItemsSource = Instructors;

                NpgsqlCommand checkCmd = Connection.GetCommand("SELECT \"Instructor\" FROM \"StudentInstructor\" WHERE \"Student\" = @studentId");
                checkCmd.Parameters.AddWithValue("@studentId", NpgsqlDbType.Integer, _fullinfoStudent.Id);
                var result = checkCmd.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    int instructorId = Convert.ToInt32(result);
                    var assignedInstructor = Instructors.FirstOrDefault(inst => inst.Id == instructorId);
                    if (assignedInstructor != null)
                    {
                        cbInstructor.SelectedItem = assignedInstructor;
                        cbInstructor.Visibility = Visibility.Visible;
                        btnAssignInstructor.Visibility = Visibility.Collapsed;
                    }
                }
                else
                {
                    cbInstructor.Visibility = Visibility.Collapsed;
                    btnAssignInstructor.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке инструкторов: {ex.Message}");
            }
        }
        private void LoadStudentInstructor()
        {
            try
            {
                NpgsqlCommand cmd = Connection.GetCommand("SELECT \"Instructor\" FROM \"StudentInstructor\" WHERE \"Student\" = @studentId");
                cmd.Parameters.AddWithValue("@studentId", NpgsqlDbType.Integer, _fullinfoStudent.Id);
                var result = cmd.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    int instructorId = Convert.ToInt32(result);
                    _fullinfoStudent.InstructorId = instructorId; 
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке назначенного инструктора: {ex.Message}");
            }
        }

            public void BindingcbGroup()
        {
            Binding binding = new Binding();
            binding.Source = Connection.classGroups;
            cbGroup.SetBinding(ItemsControl.ItemsSourceProperty, binding);
            Connection.SelectGroup();
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
        private void MWUpdateUser_Click(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrWhiteSpace(tbLogin.Text) ||
                string.IsNullOrWhiteSpace(tbPass.Text) ||
                string.IsNullOrWhiteSpace(tbFirstName.Text) ||
                string.IsNullOrWhiteSpace(tbLastName.Text) ||
                string.IsNullOrWhiteSpace(tbPhone.Text) ||
                string.IsNullOrWhiteSpace(tbEmail.Text) ||
                dpDateBirth.SelectedDate == null ||
                cbGroup.SelectedItem == null)
            {
                MessageBox.Show("Не все обязательные поля заполнены.");
                return;
            }

            var birth = dpDateBirth.SelectedDate;
            var password = tbPass.Text.Trim();
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            Instructor instructor = cbInstructor.SelectedItem as Instructor;

            ClassGroup group = cbGroup.SelectedItem as ClassGroup;
            string icon = "C:\\Users\\cloze\\source\\repos\\AutoSchoolDiplom\\AutoSchoolDiplom\\Image\\Icon.png";

            var currentYear = DateTime.Now.Year;
            var minimumBirthYear = currentYear - 16;

            if (birth.Value.Year > minimumBirthYear)
            {
                MessageBox.Show($"Ученик должен быть старше. Минимальный возраст 16. Проверьте данные.");
                return;
            }

            //try
            //{
            NpgsqlCommand cmd = Connection.GetCommand("UPDATE \"StudentGroup\" SET \"Group\"= @group where \"Student\" = @id");
            cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, _fullinfoStudent.Id);
            cmd.Parameters.AddWithValue("@group", NpgsqlDbType.Integer, group.Id);
            var result = cmd.ExecuteNonQuery();
            if (result != 0)
            {
                cmd = Connection.GetCommand("UPDATE \"Student\" SET \"Photo\"= @photo, \"Cours\" = @cours where \"Id\" = @id");
                cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, _fullinfoStudent.Id);
                cmd.Parameters.AddWithValue("@photo", NpgsqlDbType.Varchar, icon);
                cmd.Parameters.AddWithValue("@cours", NpgsqlDbType.Integer, _fullinfoStudent.Cours);
                result = cmd.ExecuteNonQuery();
                if (result != 0)
                {
                    cmd = Connection.GetCommand("UPDATE \"User\" SET \"Login\"= @login, \"Password\"= @password, \"FirstName\"= @firstName, \"LastName\"= @lastName," +
                   "\"Patronymic\"= @patronymic, \"Phone\"= @phone, \"Email\"= @email, \"DateBirth\"= @birth where \"Id\" = @id");
                    cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, _fullinfoStudent.Id);
                    cmd.Parameters.AddWithValue("@login", NpgsqlDbType.Varchar, _fullinfoStudent.Login);
                    cmd.Parameters.AddWithValue("@password", NpgsqlDbType.Varchar, hashedPassword);
                    cmd.Parameters.AddWithValue("@firstName", NpgsqlDbType.Varchar, _fullinfoStudent.FirstName);
                    cmd.Parameters.AddWithValue("@lastName", NpgsqlDbType.Varchar, _fullinfoStudent.LastName);
                    cmd.Parameters.AddWithValue("@patronymic", NpgsqlDbType.Varchar, _fullinfoStudent.Patronymic);
                    cmd.Parameters.AddWithValue("@phone", NpgsqlDbType.Varchar, _fullinfoStudent.Phone);
                    cmd.Parameters.AddWithValue("@email", NpgsqlDbType.Varchar, _fullinfoStudent.Email);
                    cmd.Parameters.AddWithValue("@birth", NpgsqlDbType.Date, birth);
                    result = cmd.ExecuteNonQuery();
                }
                if (result != 0)
                {
                    cmd = Connection.GetCommand("SELECT COUNT(*) FROM \"StudentInstructor\" WHERE \"Student\" = @studentId");
                    cmd.Parameters.AddWithValue("@studentId", NpgsqlDbType.Integer, _fullinfoStudent.Id);
                    object resultObj = cmd.ExecuteScalar();
                    if (resultObj != null)
                    {
                        if (int.TryParse(resultObj.ToString(), out int count))
                        {
                            if (count == 0)
                            {
                                cmd = Connection.GetCommand("INSERT INTO \"StudentInstructor\"(\"Student\", \"Instructor\") VALUES(@student, @instructor)");
                                cmd.Parameters.AddWithValue("@student", NpgsqlDbType.Integer, _fullinfoStudent.Id);
                                cmd.Parameters.AddWithValue("@instructor", NpgsqlDbType.Integer, instructor.Id);
                                cmd.ExecuteNonQuery();
                            }

                            else
                            {
                                cmd = Connection.GetCommand("UPDATE \"StudentInstructor\" SET \"Instructor\" = @instructor WHERE \"Student\" = @student");
                                cmd.Parameters.AddWithValue("@student", NpgsqlDbType.Integer, _fullinfoStudent.Id);
                                cmd.Parameters.AddWithValue("@instructor", NpgsqlDbType.Integer, instructor.Id);
                                cmd.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Ошибка при получении количества записей в таблице StudentInstructor.");
                        }
                    }
                    if (result != 0)
                    {
                        MessageBox.Show("Данные обновлены");
                    }
                }
            }
        }
        //}
        //catch
        //{
        //    MessageBox.Show("Выберите элемент списка");
        //    return;
        //}
       
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

        private void btnAssignInstructor_Click(object sender, RoutedEventArgs e)
        {
            cbInstructor.Visibility = Visibility.Visible;
            btnAssignInstructor.Visibility = Visibility.Collapsed;
        }
    }
}
