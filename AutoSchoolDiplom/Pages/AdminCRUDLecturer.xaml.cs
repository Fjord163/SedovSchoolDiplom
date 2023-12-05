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

    public partial class AdminCRUDLecturer : Page
    {
        public AdminCRUDLecturer()
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
    }
}
