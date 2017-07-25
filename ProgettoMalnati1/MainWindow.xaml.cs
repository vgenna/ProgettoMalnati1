﻿using System;
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

using System.Threading;

namespace ProgettoMalnati1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            startApp();
        }

        private void startApp()
        {
            clientButton.Visibility = Visibility.Hidden;
            serverButton.Visibility = Visibility.Hidden;
            button.Visibility = Visibility.Hidden;

            //this.Close();

            MessageBox.Show("WE");


        }

        private void button_Click(object sender, RoutedEventArgs e)
        {

            if (clientButton.IsChecked == true)
            {
                Client c = new Client();
                c.startBroadcastSocket();
                WinOtherUsers winOU = new WinOtherUsers(c);           

                foreach (OtherUser ou in c.otherUsers.Values)
                {
                    CheckBox cb = new CheckBox();
                    cb.Name = ou.Name;
                    cb.Content = ou.Name;
                    winOU.stackP.Children.Add(cb);
                }

                winOU.Show();
                this.Close();
                //fine client
            }
            else
            {
                if (serverButton.IsChecked == true)
                {
                    /*Server s = new Server();
                   
                    s.startBroadcastSocket();
                    
                    s.receiveFileTCP(1500);*/
                    Server s = new Server(2);
                }
            }
        }
    }
}
