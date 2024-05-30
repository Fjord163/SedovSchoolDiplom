using DBConnection;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Логика взаимодействия для EntryPage.xaml
    /// </summary>
    public partial class EntryPage : Page
    {
        public EntryPage()
        {
            InitializeComponent();

        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
           Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnLogIn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                NpgsqlCommand cmd = Connection.GetCommand("SELECT \"Id\", \"Login\", \"Password\", \"FirstName\",\"LastName\",\"Patronymic\",\"Phone\",\"Email\", \"DateBirth\", \"Role\" FROM \"User\"" +
                        "WHERE \"Login\" = @log AND \"Password\" = @pass");
                cmd.Parameters.AddWithValue("@log", NpgsqlDbType.Varchar, tbLogin.Text.Trim());
                cmd.Parameters.AddWithValue("@pass", NpgsqlDbType.Varchar, pbPassword.Password.Trim());
                NpgsqlDataReader result = cmd.ExecuteReader();


                if (result.HasRows)
                {
                    result.Read();

                    Connection.users = new CLassUser()
                    {
                        Id = result.GetInt32(0),
                        Login = result.GetString(1),
                        Password = result.GetString(2),
                        FirstName = result.GetString(3),
                        LastName = result.GetString(4),
                        Patronymic = result.GetString(5),
                        Phone = result.GetString(6),
                        Email = result.GetString(7),
                        DateBirth = result.GetDateTime(8),
                        Role = result.GetString(9)
                    };
                    result.Close();


                    switch (Connection.users.Role)
                    {
                        case "Студент":
                            NavigationService.Navigate(new AccountStudent(Connection.users));
                            break;
                        case "Инструктор":
                            NavigationService.Navigate(new AccountInstructor(Connection.users));
                            break;
                        case "Лектор":
                            NavigationService.Navigate(new AccountLector(Connection.users));
                            break;
                        case "Админ":
                            NavigationService.Navigate(new EditingLecturer());
                            break;
                    }
                }
            }
            catch
            {
                MessageBox.Show("Такого аккаунта не существует");
                return;
            }   
        }
    }
}
