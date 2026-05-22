using BusinessLaag.Interfaces;
using BusinessLaag.Managers;
using BusinessLaag.Model;
using DataLaag;
using DataLaag.Repositories;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel; 
using System.Collections.Specialized; 
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace MeerkeuzeUI
{
    public partial class MainWindow : Window
    {
       

        public MainWindow()
        {
            InitializeComponent();

           
        }

        private void BtnDocent_Click(object sender, RoutedEventArgs e)
        {
           
            DocentenWindow docentenWindow = new DocentenWindow();

            
            
            Application.Current.MainWindow = docentenWindow;

            
            docentenWindow.Show();
            this.Close();
        }

        private void BtnStudent_Click(object sender, RoutedEventArgs e)
        {
            
            StudentWindow studentWindow = new StudentWindow();
            Application.Current.MainWindow = studentWindow;
            studentWindow.Show();
            this.Close();
            
        }
    }
}