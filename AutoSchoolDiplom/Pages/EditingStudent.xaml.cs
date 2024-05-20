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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AutoSchoolDiplom.Pages
{
    public partial class EditingStudent : Page
    {
        public EditingStudent()
        {
            InitializeComponent();

            BindingLvStudents();
        }

        public void BindingLvStudents()
        {
            Binding binding = new Binding();
            binding.Source = Connection.infoStudents;
            lvStudent.SetBinding(ItemsControl.ItemsSourceProperty, binding);
            Connection.SelectInfoStudent();
        }
        private void Filter()
        {
            string searchString = tbSearch.Text.Trim();

            var view = CollectionViewSource.GetDefaultView(lvStudent.ItemsSource);
            view.Filter = new Predicate<object>(o =>
            {
                FullInfoStudent student = o as FullInfoStudent;
                if (student == null) { return false; }

                bool isVisible = true;
                if (searchString.Length > 0)
                {
                    isVisible = student.FirstName.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) != -1 ||
                        student.LastName.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) != -1 ||
                        student.Patronymic.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) != -1 ||
                        student.Login.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) != -1;
                }
                return isVisible;
            });
        }
        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            Filter();
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }
        private void btnTransitionPageEditLecturer_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new EditingInstructor());
            Connection.infoStudents.Clear();
        }
        private void btnTransitionPageEditStudent_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Вы уже находитесь на данной странице");
        }
        private void btnTransitionPageEditInstructor_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new EditingInstructor());
            Connection.infoStudents.Clear();
        }
        private void btnAddNewStudent_Click(object sender, RoutedEventArgs e)
        {
            ModalWindow.MWAddStudent mwAddStudent = new ModalWindow.MWAddStudent();
            bool? res = mwAddStudent.ShowDialog();
            if (res == false)
            {
                Connection.infoStudents.Clear();
                BindingLvStudents();
            }
        }
        private void DeleteItemLvStudent(object sender, RoutedEventArgs e)
        {
            try
            {
                FullInfoStudent student = (sender as Button)?.DataContext as FullInfoStudent;

                NpgsqlCommand cmd = Connection.GetCommand("DELETE FROM \"StudentGroup\" WHERE \"Student\" = @id returning \"Student\"");
                cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, student.Student);
                cmd.Parameters.AddWithValue("@group", NpgsqlDbType.Integer, student.Group);
                var result = cmd.ExecuteScalar();
                if (result != null)
                {
                    int IdStudent = student.Student = (int)result;

                    cmd = Connection.GetCommand("DELETE FROM \"Student\" WHERE \"Id\" = @id returning \"Id\"");
                    cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, student.Student);
                    cmd.Parameters.AddWithValue("@photo", NpgsqlDbType.Varchar, student.Photo);
                    cmd.Parameters.AddWithValue("@cours", NpgsqlDbType.Integer, student.Cours);
                    result = cmd.ExecuteNonQuery();
                    if (result != null)
                    {
                        cmd = Connection.GetCommand("DELETE FROM \"User\" WHERE \"Id\" = @id");
                        cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, IdStudent);
                        cmd.Parameters.AddWithValue("@login", NpgsqlDbType.Varchar, student.Login);
                        cmd.Parameters.AddWithValue("@password", NpgsqlDbType.Varchar, student.Password);
                        cmd.Parameters.AddWithValue("@firstName", NpgsqlDbType.Varchar, student.FirstName);
                        cmd.Parameters.AddWithValue("@lastName", NpgsqlDbType.Varchar, student.LastName);
                        cmd.Parameters.AddWithValue("@patronymic", NpgsqlDbType.Varchar, student.Patronymic);
                        cmd.Parameters.AddWithValue("@phone", NpgsqlDbType.Varchar, student.Phone);
                        cmd.Parameters.AddWithValue("@email", NpgsqlDbType.Varchar, student.Email);
                        cmd.Parameters.AddWithValue("@dateBirth", NpgsqlDbType.Date, student.DateBirth);
                        cmd.Parameters.AddWithValue("@role", NpgsqlDbType.Varchar, student.Role);
                        result = cmd.ExecuteNonQuery();
                    }
                    if (result != null)
                    {
                        Connection.infoStudents.Remove(lvStudent.SelectedItem as FullInfoStudent);
                        Connection.infoStudents.Clear();
                        BindingLvStudents();
                    }
                    if (result == null) { MessageBox.Show("Данные не удалены"); }
                    else { MessageBox.Show("Данные удалены"); }
                }
            } 
            catch 
            {
                MessageBox.Show("Произошла ошибка");
            }
        }
        private void EditItemLvStudent(object sender, RoutedEventArgs e)
        {
            var comboBoxItems = Connection.infoStudents.Select(ul => new StudentComboBoxItem
            {
                Category = ul.Category,
                TheoryHours = ul.TheoryHours,
                DrivingHours = ul.DrivingHours
            }).ToList();

            FullInfoStudent student = (sender as Button)?.DataContext as FullInfoStudent;
            StudentComboBoxItem selectedComboBoxItem = null;
            if (student != null) {
                selectedComboBoxItem = comboBoxItems.FirstOrDefault(item => item.Category == student.Category && item.TheoryHours == student.TheoryHours && item.DrivingHours == student.DrivingHours);
            }

            ModalWindow.MWEditStudent mwEditStudent = new ModalWindow.MWEditStudent(student, comboBoxItems, selectedComboBoxItem);
            bool? res = mwEditStudent.ShowDialog();
            if (res == false)
            {
                Connection.infoStudents.Clear();
                BindingLvStudents();
            }
        }
    }
}
