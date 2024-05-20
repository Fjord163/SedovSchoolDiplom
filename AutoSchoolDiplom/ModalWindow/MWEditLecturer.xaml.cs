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
            try
            {

                var birth = dpDateBirth.SelectedDate;
                var dateEmployment = dpDateEmployment.SelectedDate;
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
                    cmd.Parameters.AddWithValue("@password", NpgsqlDbType.Varchar, _fullinfoLecturer.Password);
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
    }
}
