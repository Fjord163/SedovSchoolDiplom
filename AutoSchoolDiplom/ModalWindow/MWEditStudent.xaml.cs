using AutoSchoolDiplom.Pages;
using DBConnection;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
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
        public MWEditStudent(FullInfoStudent fullInfoStudent, List<StudentComboBoxItem> comboBoxItems, StudentComboBoxItem selectedComboBoxItem)
        {
            InitializeComponent();

            _fullinfoStudent = fullInfoStudent;
            DataContext = fullInfoStudent;

            cbCours.ItemsSource = comboBoxItems;


            if (comboBoxItems != null && comboBoxItems.Contains(selectedComboBoxItem)) { 
                cbCours.SelectedItem = selectedComboBoxItem;
            }
            BindingcbCours();
            BindingcbGroup();
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
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
            Connection.classGroups.Clear();
            Connection.cours.Clear();
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
        private void MWUpdateUser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string icon = "C:\\Users\\cloze\\source\\repos\\AutoSchoolDiplom\\AutoSchoolDiplom\\Image\\Icon.png";

                NpgsqlCommand cmd = Connection.GetCommand("UPDATE \"StudentGroup\" SET \"Group\"= @group where \"Student\" = @id");
                cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, _fullinfoStudent.Id);
                cmd.Parameters.AddWithValue("@group", NpgsqlDbType.Integer, _fullinfoStudent.Group);
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
                        cmd.Parameters.AddWithValue("@password", NpgsqlDbType.Varchar, _fullinfoStudent.Password);
                        cmd.Parameters.AddWithValue("@firstName", NpgsqlDbType.Varchar, _fullinfoStudent.FirstName);
                        cmd.Parameters.AddWithValue("@lastName", NpgsqlDbType.Varchar, _fullinfoStudent.LastName);
                        cmd.Parameters.AddWithValue("@patronymic", NpgsqlDbType.Varchar, _fullinfoStudent.Patronymic);
                        cmd.Parameters.AddWithValue("@phone", NpgsqlDbType.Varchar, _fullinfoStudent.Phone);
                        cmd.Parameters.AddWithValue("@email", NpgsqlDbType.Varchar, _fullinfoStudent.Email);
                        cmd.Parameters.AddWithValue("@birth", NpgsqlDbType.Date, _fullinfoStudent.DateBirth);
                        result = cmd.ExecuteNonQuery();
                    }
                }
                if (result != 0)
                {
                    MessageBox.Show("Данные обновлены");
                }
            }
            catch
            {
                MessageBox.Show("Выберите элемент списка");
                return;
            }
        }
    }
}
