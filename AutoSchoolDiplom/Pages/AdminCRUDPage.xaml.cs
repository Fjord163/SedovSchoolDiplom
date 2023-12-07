﻿using DBConnection;
using Npgsql;
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


    public partial class AdminCRUDPage : Page
    {
        public AdminCRUDPage()
        {
            InitializeComponent();

            BindingLvStudents();

            NameUser.Text = Connection.users.FirstName + " " + Connection.users.LastName + " " + Connection.users.Patronymic;

        }

        public void BindingLvStudents()
        {
            Binding binding = new Binding();
            binding.Source = Connection.infoStudents;
            lvStudents.SetBinding(ItemsControl.ItemsSourceProperty, binding);
            Connection.SelectInfoStudent();
        }

        private void Filter()
        {
            string searchString = Search.Text.Trim();

            var view = CollectionViewSource.GetDefaultView(lvStudents.ItemsSource);
            view.Filter = new Predicate<object>(o =>
            {
                FullInfoStudent product = o as FullInfoStudent;
                if (product == null) { return false; }

                bool isVisible = true;
                if (searchString.Length > 0)
                {
                    isVisible = product.FirstName.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) != -1 ||
                        product.LastName.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) != -1;
                }

                //isVisible = isVisible && category == product.Category;

                return isVisible;
            });
        }

        private void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            Filter();
        }

        private void btnTransitionInstructor_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AdminCRUDInctructor());
            Connection.infoStudents.Clear();
        }

        private void btnTransitionLecturer_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AdminCRUDLecturer());
            Connection.infoStudents.Clear();
        }

        private void btnTransitionStudent_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Вы уже находитесь на данной странице");
        }

        private void btnSignOut_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new EntryPage());
            Connection.infoStudents.Clear();
        }
    }
}
