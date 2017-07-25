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

using System.Threading;
using System.Diagnostics;

namespace ProgettoMalnati1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string s;
        public MainWindow()
        {
            InitializeComponent();
            string[] args = Environment.GetCommandLineArgs();
            s = args[1];
            //startApp();  //commenta per eseguire la versione di test GESTIRE GLI ARGOMENTI SBAGLIATI
        }

        private void startApp()
        {
            clientButton.Visibility = Visibility.Hidden;
            serverButton.Visibility = Visibility.Hidden;
            button.Visibility = Visibility.Hidden;

            //this.Close();

            string[] args = Environment.GetCommandLineArgs();

            if (args.Length > 1)
            {
                if (args[1] == "server")
                {
                    //codice server
                    MessageBox.Show("Serverrrrrr");
                    ServerRoutine();
                }

            }
            else
            {
                Process.Start("ProgettoMalnati1.exe", "server");
                //mettere in attesa dell'evento condividi
                ClientRoutine();
            }
                
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {

            if (clientButton.IsChecked == true)
            {
                ClientRoutine();
            }
            else
            {
                if (serverButton.IsChecked == true)
                {

                    ServerRoutine();
                }
            }
        }

        public void ServerRoutine()
        {
            /*Server s = new Server();
                   
              s.startBroadcastSocket();
                
              s.receiveFileTCP(1500);*/
            Server s = new Server(2);
        }

        public void ClientRoutine()
        {
            Client c = new Client(s);
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
    }
}
