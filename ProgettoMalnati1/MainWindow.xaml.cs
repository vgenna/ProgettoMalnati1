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
            startApp();  //commenta per eseguire la versione di test
            this.Close();
        }

        private void startApp()
        {
            //clientButton.Visibility = Visibility.Hidden;
            //serverButton.Visibility = Visibility.Hidden;
            //button.Visibility = Visibility.Hidden;

            //this.Close();

            string[] args = Environment.GetCommandLineArgs();

            /*foreach (string arg in args)
                MessageBox.Show(arg);
            if (args.Length == 2)
                Process.Start(args[0], "\""+args[1]+"\""+" server");*/
            try
            {
                if (args.Length == 3)
                {
                    //chiamato dalla condivisione del file (file o cartella su cui ha fatto clic dx passato come args[1])
                    if (args[2] == "client")
                    {
                        s = args[1];
                        //MessageBox.Show("Apro il client: "+s);
                        ClientRoutine();
                    }
                    else
                    {
                        //ERRORE NELL'APPLICAZIONE
                        throw new Exception("Errore inatteso nell'applicazione (argomenti errati)");
                    }
                }
                else
                {
                    if (args.Length == 1)
                    {
                        //sta lanciando l'applicazione per la prima volta, devo aprire solo il server

                        //MessageBox.Show("Apro il server");
                        Process.Start(args[0], "server");
                    }
                    else
                    {
                        if (args.Length == 2)
                        {
                            if (args[1] == "server")
                            {
                                //MessageBox.Show("Lancio il server");
                                ServerRoutine(); //chiede il settaggio delle impostazioni
                            }
                            else
                            {
                                //errore nell'applicazione
                                throw new Exception("Errore inatteso nell'applicazione (argomenti errati)");
                            }
                        }
                    }
                }
            } catch (Exception e)
            {
                MessageBox.Show(e.Message);
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
            //oSignalEvent = new ManualResetEvent(false);
            ImpostazioniPrimoAvvio impostazioni = new ImpostazioniPrimoAvvio();
            impostazioni.Show();//mostra finestra

            //oSignalEvent.WaitOne(); //This thread will block here until the reset event is sent.
            //oSignalEvent.Reset();
            //Server s = new Server(2);
        }

        public void ClientRoutine()
        {
            Client c = new Client(s);
            c.startBroadcastSocket();
            WinOtherUsers winOU = new WinOtherUsers(c);

            foreach (OtherUser ou in c.otherUsers.Values)
            {
                CheckBox cb = new CheckBox();
                Style style = this.FindResource("myCheckboxStyle") as Style;
                cb.Style = style;
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
