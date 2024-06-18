using DBConnection;
using Npgsql;
using NpgsqlTypes;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using static MaterialDesignThemes.Wpf.Theme.ToolBar;

namespace AutoSchoolDiplom.Pages
{
    /// <summary>
    /// Логика взаимодействия для EditingLecturer.xaml
    /// </summary>
    public partial class EditingLecturer : Page
    {
        public EditingLecturer()
        {
            InitializeComponent();

            BindingLvLecturers();

        }

        public void BindingLvLecturers()
        {
            Binding binding = new Binding();
            binding.Source = Connection.infoLecturers;
            lvLecturer.SetBinding(ItemsControl.ItemsSourceProperty, binding);
            Connection.SelectInfoLecturers();
        }
        private void Filter()
        {
            string searchString = tbSearch.Text.Trim();

            var view = CollectionViewSource.GetDefaultView(lvLecturer.ItemsSource);
            view.Filter = new Predicate<object>(o =>
            {
                FullInfoLecturer lecturer = o as FullInfoLecturer;
                if (lecturer == null) { return false; }

                bool isVisible = true;
                if (searchString.Length > 0)
                {
                    isVisible = lecturer.FirstName.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) != -1 ||
                        lecturer.LastName.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) != -1 ||
                        lecturer.Patronymic.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) != -1 ||
                        lecturer.Login.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) != -1;
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

        private void btnTransitionPageEditStudent_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new EditingStudent());
            Connection.infoLecturers.Clear();

        }

        private void btnTransitionPageEditLecturer_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Вы уже находитесь на данной странице");
        }

        private void btnTransitionPageEditInstructor_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new EditingInstructor());
            Connection.infoLecturers.Clear();
        }

        private void AddNewLecturer_Click(object sender, RoutedEventArgs e)
        {
            ModalWindow.MWAddLecturer mwAddLecturer = new ModalWindow.MWAddLecturer();
            bool? res = mwAddLecturer.ShowDialog();
            if(res == false)
            {
                Connection.infoLecturers.Clear();
                BindingLvLecturers();
            }
        }

        private void DeleteItemLvLecturer(object sender, RoutedEventArgs e)
        {
            FullInfoLecturer lecturer = (sender as Button)?.DataContext as FullInfoLecturer;

            NpgsqlCommand cmd = Connection.GetCommand("DELETE FROM \"Lecturer\" WHERE \"Id\" = @id returning \"Id\"");
            cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, lecturer.Id);
            cmd.Parameters.AddWithValue("@dateEmployment", NpgsqlDbType.Date, lecturer.DateEmployment);
            var result = cmd.ExecuteScalar();
            if (result != null)
            {
                int IdLecturer = lecturer.Id = (int)result;

                cmd = Connection.GetCommand("DELETE FROM \"User\" WHERE \"Id\" = @id");
                cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, IdLecturer);
                cmd.Parameters.AddWithValue("@login", NpgsqlDbType.Varchar, lecturer.Login);
                cmd.Parameters.AddWithValue("@password", NpgsqlDbType.Varchar, lecturer.Password);
                cmd.Parameters.AddWithValue("@firstName", NpgsqlDbType.Varchar, lecturer.FirstName);
                cmd.Parameters.AddWithValue("@lastName", NpgsqlDbType.Varchar, lecturer.LastName);
                cmd.Parameters.AddWithValue("@patronymic", NpgsqlDbType.Varchar, lecturer.Patronymic);
                cmd.Parameters.AddWithValue("@phone", NpgsqlDbType.Varchar, lecturer.Phone);
                cmd.Parameters.AddWithValue("@email", NpgsqlDbType.Varchar, lecturer.Email);
                cmd.Parameters.AddWithValue("@dateBirth", NpgsqlDbType.Date, lecturer.DateBirth);
                cmd.Parameters.AddWithValue("@role", NpgsqlDbType.Varchar, lecturer.Role);
                result = cmd.ExecuteNonQuery();
                if (result != null)
                {
                    Connection.infoLecturers.Remove(lvLecturer.SelectedItem as FullInfoLecturer);
                    Connection.infoLecturers.Clear();
                    BindingLvLecturers();
                }
                if (result == null) { MessageBox.Show("Данные не удалены"); }
                else { MessageBox.Show("Данные удалены"); }
            }
        }

        private void EditItemLvLecturer(object sender, RoutedEventArgs e)
        {
            FullInfoLecturer lecturer = (sender as Button)?.DataContext as FullInfoLecturer;

            ModalWindow.MWEditLecturer mwEditLecturer = new ModalWindow.MWEditLecturer(lecturer);
            mwEditLecturer.ShowDialog();
        }

        private void btnTimetableForming_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new FormingTimeTable());
            Connection.infoLecturers.Clear();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Connection.users = null;

            NavigationService.Navigate(new EntryPage());
        }
    }
}
