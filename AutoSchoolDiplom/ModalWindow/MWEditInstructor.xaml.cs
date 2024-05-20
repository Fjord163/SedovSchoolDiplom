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
    /// Логика взаимодействия для MWEditInstructor.xaml
    /// </summary>
    public partial class MWEditInstructor : Window
    {
        private FullInfoInstructor _fullinfoInstructor;

        public MWEditInstructor(FullInfoInstructor fullInfoInstructor)
        {
            InitializeComponent();

            _fullinfoInstructor = fullInfoInstructor;
            DataContext = fullInfoInstructor;
        }

        private void btnUpdateUser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                NpgsqlCommand cmd = Connection.GetCommand("UPDATE \"Instructor\" SET \"DateEmployment\"= @dateEmployment, \"DrivingExperience\" = @drivingExperience where \"Id\" = @id");
                cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, _fullinfoInstructor.Id);
                cmd.Parameters.AddWithValue("@dateEmployment", NpgsqlDbType.Date, _fullinfoInstructor.DateEmployment);
                cmd.Parameters.AddWithValue("@drivingExperience", NpgsqlDbType.Varchar, _fullinfoInstructor.DrivingExperience);
                var result = cmd.ExecuteNonQuery();
                if (result != 0)
                {
                    cmd = Connection.GetCommand("UPDATE \"User\" SET \"Login\"= @login, \"Password\"= @password, \"FirstName\"= @firstName, \"LastName\"= @lastName," +
                        "\"Patronymic\"= @patronymic, \"Phone\"= @phone, \"Email\"= @email, \"DateBirth\"= @birth where \"Id\" = @id");
                    cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, _fullinfoInstructor.Id);
                    cmd.Parameters.AddWithValue("@login", NpgsqlDbType.Varchar, _fullinfoInstructor.Login);
                    cmd.Parameters.AddWithValue("@password", NpgsqlDbType.Varchar, _fullinfoInstructor.Password);
                    cmd.Parameters.AddWithValue("@firstName", NpgsqlDbType.Varchar, _fullinfoInstructor.FirstName);
                    cmd.Parameters.AddWithValue("@lastName", NpgsqlDbType.Varchar, _fullinfoInstructor.LastName);
                    cmd.Parameters.AddWithValue("@patronymic", NpgsqlDbType.Varchar, _fullinfoInstructor.Patronymic);
                    cmd.Parameters.AddWithValue("@phone", NpgsqlDbType.Varchar, _fullinfoInstructor.Phone);
                    cmd.Parameters.AddWithValue("@email", NpgsqlDbType.Varchar, _fullinfoInstructor.Email);
                    cmd.Parameters.AddWithValue("@birth", NpgsqlDbType.Date, _fullinfoInstructor.DateBirth);
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
