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
    /// Логика взаимодействия для AccountStudent.xaml
    /// </summary>
    public partial class AccountStudent : Page
    {
        public AccountStudent()
        {
            InitializeComponent();

            Uri path = new Uri("https://gas-kvas.com/grafic/uploads/posts/2023-09/1695869715_gas-kvas-com-p-kartinki-bez-13.png");
            if(PhotoAccount.ImageSource == null)
            {
                PhotoAccount.ImageSource = new BitmapImage(path);
            }
        }


    }
}
