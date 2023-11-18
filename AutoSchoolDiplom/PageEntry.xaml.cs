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

namespace AutoSchoolDiplom
{
    public partial class PageEntry : Page
    {
        public PageEntry()
        {
            InitializeComponent();

            if (tbLogin.Text.Trim() == "")
            {
                tbLogin.Text = "Введите логин";
                tbLogin.Opacity = 0.5;
            }
            if (tbPassword.Text.Trim() == "")
            {
                tbPassword.Text = "Введите пароль";
                tbPassword.Opacity = 0.5;
            }

        }

        private void btnSignIn_Click(object sender, RoutedEventArgs e)
        {
            var role = cbRole.Text.Trim();

            switch (role)
            {
                case "Инструктор":
                    NpgsqlCommand cmd = Connection.GetCommand("SELECT \"Id\",\"FirstName\",\"LastName\",\"Patronymic\",\"Password\",\"Phone\",\"Email\", \"Login\" FROM \"Instructor\"" +
                            "WHERE \"Login\" = @log AND \"Password\" = @pass");
                    cmd.Parameters.AddWithValue("@log", NpgsqlDbType.Varchar, tbLogin.Text.Trim());
                    cmd.Parameters.AddWithValue("@pass", NpgsqlDbType.Varchar, tbPassword.Text.Trim());
                    NpgsqlDataReader result = cmd.ExecuteReader();

                    if (result.HasRows)
                    {
                        result.Read();
                        Connection.instructor = new ClassInstructor()
                        {
                            FirstName = result.GetString(1),
                            LastName = result.GetString(2),
                            Patronymic = result.GetString(3),
                            Password = result.GetString(4),
                            Phone = result.GetString(5),
                            Email = result.GetString(6),
                            Login = result.GetString(7)
                        };
                        MessageBox.Show("Зашел инструкотор");
                        result.Close();
                    }
                    else { MessageBox.Show("не выполнено"); }
                    break;
                case "Лектор":
                     cmd = Connection.GetCommand("SELECT \"Id\",\"FirstName\",\"LastName\",\"Patronymic\",\"Password\",\"Phone\",\"Email\", \"Login\" FROM \"Lecturer\"" +
                            "WHERE \"Login\" = @log AND \"Password\" = @pass");
                    cmd.Parameters.AddWithValue("@log", NpgsqlDbType.Varchar, tbLogin.Text.Trim());
                    cmd.Parameters.AddWithValue("@pass", NpgsqlDbType.Varchar, tbPassword.Text.Trim());
                    result = cmd.ExecuteReader();

                    if (result.HasRows)
                    {
                        result.Read();
                        Connection.lecturer = new ClassLecturer()
                        {
                            FirstName = result.GetString(1),
                            LastName = result.GetString(2),
                            Patronymic = result.GetString(3),
                            Password = result.GetString(4),
                            Phone = result.GetString(5),
                            Email = result.GetString(6),
                            Login = result.GetString(7)
                        };
                        MessageBox.Show("Зашел лектор");
                        result.Close();
                    }
                    else { MessageBox.Show("не выполнено"); }
                    break;
                case "Ученик":
                    cmd = Connection.GetCommand("SELECT \"Id\",\"FirstName\",\"LastName\",\"Patronymic\",\"Password\",\"Phone\",\"Email\",\"Photo\",\"Cours\", \"Login\" FROM \"Student\"" +
                           "WHERE \"Login\" = @log AND \"Password\" = @pass");
                    cmd.Parameters.AddWithValue("@log", NpgsqlDbType.Varchar, tbLogin.Text.Trim());
                    cmd.Parameters.AddWithValue("@pass", NpgsqlDbType.Varchar, tbPassword.Text.Trim());
                    result = cmd.ExecuteReader();

                    if (result.HasRows)
                    {
                        result.Read();
                        Connection.student = new ClassStudent()
                        {
                            FirstName = result.GetString(1),
                            LastName = result.GetString(2),
                            Patronymic = result.GetString(3),
                            Password = result.GetString(4),
                            Phone = result.GetString(5),
                            Email = result.GetString(6),
                            Photo = result.GetString(7),
                            Cours = result.GetInt32(8),
                            Login = result.GetString(9)
                        };
                        MessageBox.Show("Зашел Ученик");
                        result.Close();
                    }
                    else { MessageBox.Show("не выполнено"); }
                    break;
            }

            if (tbLogin.Text.Trim() == "")
            {
                tbLogin.Text = "Введите логин";
                tbLogin.Opacity = 0.5;
            }
            if (tbPassword.Text.Trim() == "")
            {
                tbPassword.Text = "Введите пароль";
                tbPassword.Opacity = 0.5;
            }
        }

        private void tbLogin_GotFocus(object sender, RoutedEventArgs e)
        {
            if (tbLogin.Text.Trim() == "Введите логин")
            {
                tbLogin.Clear();
                tbLogin.Opacity = 1;
            }

            if (tbPassword.Text.Trim() == "")
            {
                tbPassword.Text = "Введите пароль";
                tbPassword.Opacity = 0.5;
            }
        }

        private void tbPassword_GotFocus(object sender, RoutedEventArgs e)
        {
            if (tbPassword.Text.Trim() == "Введите пароль")
            {
                tbPassword.Clear();
                tbPassword.Opacity = 1;
            }

            if (tbLogin.Text.Trim() == "")
            {
                tbLogin.Text = "Введите логин";
                tbLogin.Opacity = 0.5;
            }
        }
    }
}
