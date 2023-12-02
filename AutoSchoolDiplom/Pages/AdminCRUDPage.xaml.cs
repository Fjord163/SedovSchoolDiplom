using DBConnection;
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
    /// Логика взаимодействия для AdminCRUDPage.xaml
    /// </summary>
    public partial class AdminCRUDPage : Page
    {
        public AdminCRUDPage()
        {
            InitializeComponent();

            binding();
        }

        public void binding()
        {
            Binding binding = new Binding();
            binding.Source = Connection.Users;
            lv.SetBinding(ItemsControl.ItemsSourceProperty, binding);
            Connection.Select();
        }
    }
}
