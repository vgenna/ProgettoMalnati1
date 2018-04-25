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

            /****************start******************/
            /*WrapPanel p = new WrapPanel();
            p.VerticalAlignment = VerticalAlignment.Top;
            p.Margin = new Thickness(7.0, 7.0, 7.0, 0);
            p.Width = 290;
            p.Height = 290;
            p.Background = Brushes.White;
            p.Orientation = Orientation.Vertical;

            for (var i = 0; i < 3; i++)
            {
                p.Children.Add(new Rectangle
                {
                    Width = 100,
                    Height = 20,
                    StrokeThickness = 1,
                    Stroke = new SolidColorBrush(Colors.Black),
                    Margin = new Thickness(5)
                });
            }
            winOU.stackP.Children.Add(p);*/
            

            Image im2 = new Image();
            im2.Height = 96.00;
            im2.Width = 96.00;
            var uriSource2 = new Uri(AppDomain.CurrentDomain.BaseDirectory + "\\utente.ico");
            im2.Source = new BitmapImage(uriSource2);
            //im2.Stretch = System.Windows.Media.Stretch.None;
            
            /******************stop****************/
            foreach (OtherUser ou in c.otherUsers.Values)
            {

                
                CheckBox cb = new CheckBox();
                Style style = this.FindResource("myCheckboxStyle") as Style;
                cb.Style = style;
                cb.Name = ou.Name;
                //cb.Content = ou.Name;
                //creo il pannello che conterrà immagine e nome dell'utente
                WrapPanel p = new WrapPanel();
                p.VerticalAlignment = VerticalAlignment.Top;
                p.Margin = new Thickness(7.0, 7.0, 7.0, 0);
                p.Width = 290;
                p.Height = 290;
                p.Background = Brushes.Aqua;
                p.Orientation = Orientation.Vertical;
                TextBlock tb = new TextBlock();
                tb.Text = ou.Name;
                p.Children.Add(tb);
                p.Children.Add(im2);
                ////////////////////////////////////////////////////////////
                cb.Content = p;//cb.Content = im2;
                winOU.stackP.Children.Add(cb);
           }
            Image im3 = new Image();
            im3.Height = 96.00;
            im3.Width = 96.00;
            var uriSource3 = new Uri(AppDomain.CurrentDomain.BaseDirectory + "\\utente.ico");
            im3.Source = new BitmapImage(uriSource3);
            //im3.Stretch = System.Windows.Media.Stretch.None;
            for (var i = 0; i<1; i++)
           {
                CheckBox cb = new CheckBox();
                Style style = this.FindResource("myCheckboxStyle") as Style;
                cb.Style = style;
                //cb.Content = ou.Name;
                //creo il pannello che conterrà immagine e nome dell'utente
                WrapPanel p = new WrapPanel();
                p.VerticalAlignment = VerticalAlignment.Top;
                p.Margin = new Thickness(7.0, 7.0, 7.0, 0);
                p.Width = 290;
                p.Height = 290;
                p.Background = Brushes.Aqua;
                p.Orientation = Orientation.Vertical;
                TextBlock tb = new TextBlock();
                tb.Text = "secondo utente";//ou.Name;
                p.Children.Add(tb);
                p.Children.Add(im3);
                cb.Content = p;
                winOU.stackP.Children.Add(cb);
           }

            winOU.Show();
            this.Close();
            //fine client
        }
    }
}
