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
    /// <summary>
    /// Логика взаимодействия для EditingInstructor.xaml
    /// </summary>
    public partial class EditingInstructor : Page
    {
        public EditingInstructor()
        {
            InitializeComponent();

            BindingLvInstructors();
        }

        public void BindingLvInstructors()
        {
            Binding binding = new Binding();
            binding.Source = Connection.infoInstructors;
            lvInstructor.SetBinding(ItemsControl.ItemsSourceProperty, binding);
            Connection.SelectInfoInstructors();
        }
        private void Filter()
        {
            string searchString = tbSearch.Text.Trim();

            var view = CollectionViewSource.GetDefaultView(lvInstructor.ItemsSource);
            view.Filter = new Predicate<object>(o =>
            {
                FullInfoInstructor instructor = o as FullInfoInstructor;
                if (instructor == null) { return false; }

                bool isVisible = true;
                if (searchString.Length > 0)
                {
                    isVisible = instructor.FirstName.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) != -1 ||
                        instructor.LastName.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) != -1 ||
                        instructor.Patronymic.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) != -1 ||
                        instructor.Login.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) != -1;
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
            NavigationService.Navigate(new EditingLecturer());
            Connection.infoInstructors.Clear();
        }
        private void btnTransitionPageEditStudent_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new EditingStudent());
            Connection.infoInstructors.Clear();
        }
        private void btnTransitionPageEditInstructor_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Вы уже находитесь на данной странице");
        }
        private void DeleteItemLvinstructor(object sender, RoutedEventArgs e)
        {
            FullInfoInstructor instructor = (sender as Button)?.DataContext as FullInfoInstructor;

            NpgsqlCommand cmd = Connection.GetCommand("delete from \"Instructor\" where \"Id\" = @id returning \"Id\"");
            cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, instructor.Id);
            cmd.Parameters.AddWithValue("@dateEmployment", NpgsqlDbType.Date, instructor.DateEmployment);
            cmd.Parameters.AddWithValue("@drivingExperience", NpgsqlDbType.Varchar, instructor.DrivingExperience);
            var result = cmd.ExecuteScalar();
            if (result != null)
            {
                int instructorId = instructor.Id = (int)result;

                cmd = Connection.GetCommand("DELETE FROM \"User\" WHERE \"Id\" = @id");
                cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, instructorId);
                cmd.Parameters.AddWithValue("@login", NpgsqlDbType.Varchar, instructor.Login);
                cmd.Parameters.AddWithValue("@password", NpgsqlDbType.Varchar, instructor.Password);
                cmd.Parameters.AddWithValue("@firstName", NpgsqlDbType.Varchar, instructor.FirstName);
                cmd.Parameters.AddWithValue("@lastName", NpgsqlDbType.Varchar, instructor.LastName);
                cmd.Parameters.AddWithValue("@patronymic", NpgsqlDbType.Varchar, instructor.Patronymic);
                cmd.Parameters.AddWithValue("@phone", NpgsqlDbType.Varchar, instructor.Phone);
                cmd.Parameters.AddWithValue("@email", NpgsqlDbType.Varchar, instructor.Email);
                cmd.Parameters.AddWithValue("@dateBirth", NpgsqlDbType.Date, instructor.DateBirth);
                cmd.Parameters.AddWithValue("@role", NpgsqlDbType.Varchar, instructor.Role);
                result = cmd.ExecuteNonQuery();
                if (result != null)
                {
                    Connection.infoInstructors.Remove(lvInstructor.SelectedItem as FullInfoInstructor);
                    Connection.infoInstructors.Clear();
                    BindingLvInstructors();
                }

                if (result == null) { MessageBox.Show("Данные не удалены"); }
                else { MessageBox.Show("Данные удалены"); }
            }
        }
        private void EditItemLvInstructor(object sender, RoutedEventArgs e)
        {
            FullInfoInstructor instructor = (sender as Button)?.DataContext as FullInfoInstructor;

            ModalWindow.MWEditInstructor mwWditInstructor = new ModalWindow.MWEditInstructor(instructor);
            bool? res = mwWditInstructor.ShowDialog();
            if (res == false)
            {
                Connection.infoInstructors.Clear();
                BindingLvInstructors();
            }
        }
        private void btnAddNewInstructor_Click(object sender, RoutedEventArgs e)
        {
            ModalWindow.MWAddInstructor mwAddInstructor = new ModalWindow.MWAddInstructor();
            bool? res = mwAddInstructor.ShowDialog();
            if (res == false)
            {
                Connection.infoInstructors.Clear();
                BindingLvInstructors();
            }
        }

        private void btnTimetableForming_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new FormingTimeTable());
            Connection.infoInstructors.Clear();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Connection.users = null;

            NavigationService.Navigate(new EntryPage());
        }
    }
}
