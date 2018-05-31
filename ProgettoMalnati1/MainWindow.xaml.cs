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

using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Net;
using System.Windows.Forms;

using System.Configuration;

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
            // will become true if there is another instance running of the same application.
            /*bool exists = Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location)).Count() > 1;
            if (exists) {
                MessageBox.Show("Hai selezionato più di un file");
                return;
            }*/
          
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
                        //ConfigurationManager.AppSettings["nome"] = "nino";
                        System.Windows.Forms.MessageBox.Show("Nome questo client: " + ConfigurationManager.AppSettings.Get("nome"));
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
                        Process.Start(args[0], "server"); //il processo lancia il server e si chiude
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
                System.Windows.MessageBox.Show(e.Message);
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
            //myname = impostazioni.cname;

            //oSignalEvent.WaitOne(); //This thread will block here until the reset event is sent.
            //oSignalEvent.Reset();
            //Server s = new Server(2);
        }

        public void ClientRoutine()
        {
            bool connected = checkConnection();
            if (connected == false) {
                System.Windows.Forms.MessageBox.Show("Connessione internet assente", "Errore connessione internet", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Client c = new Client(s);
            c.startBroadcastSocket();
            WinOtherUsers winOU = new WinOtherUsers(c);

            /****************start******************/
            /*WrapPanel p2 = new WrapPanel();
            p2.VerticalAlignment = VerticalAlignment.Top;
            p2.Margin = new Thickness(7.0, 7.0, 7.0, 0);
            p2.Width = 290;
            p2.Height = 290;
            p2.Background = Brushes.White;
            p2.Orientation = Orientation.Vertical;

            for (var i = 0; i < 3; i++)
            {
                p2.Children.Add(new Rectangle
                {
                    Width = 100,
                    Height = 20,
                    StrokeThickness = 1,
                    Stroke = new SolidColorBrush(Colors.Black),
                    Margin = new Thickness(5)
                });
            }
            winOU.stackP.Children.Add(p2);*/
            

            
            
            //im2.Stretch = System.Windows.Media.Stretch.None;
            
            /******************stop****************/
            foreach (OtherUser ou in c.otherUsers.Values)
            {
                System.Windows.Controls.CheckBox cb = new System.Windows.Controls.CheckBox();
                //Style style = this.FindResource("myCheckboxStyle") as Style;
                //cb.Style = style;
                cb.Name = ou.Name;

                var memory = new MemoryStream();
                ou.Image.Save(memory, System.Drawing.Imaging.ImageFormat.Jpeg);
                memory.Position = 0;
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                
                Image im2 = new Image();
                im2.Height = bitmapImage.Height; //farle più piccole in proporzione
                im2.Width = bitmapImage.Width;
                im2.Source = bitmapImage;

                //cb.Content = ou.Name;
                //creo il pannello che conterrà immagine e nome dell'utente
                WrapPanel p = new WrapPanel();
                TextBlock tb = new TextBlock();
                p.VerticalAlignment = VerticalAlignment.Top;
                p.Margin = new Thickness(7.0, 7.0, 7.0, 0);
                p.Width = im2.Width;
                p.Height = im2.Height + tb.Height;
                p.Background = Brushes.LightGreen;
                p.Orientation = System.Windows.Controls.Orientation.Horizontal;
                
                tb.Inlines.Add(new Bold(new Run(ou.Name)));
                //tb.Text = ou.Name;
                p.Children.Add(tb);
                p.Children.Add(im2);
                ////////////////////////////////////////////////////////////
                cb.Content = p;//cb.Content = im2;
                winOU.stackP.Children.Add(cb);
           }
            winOU.Show();
            this.Close();
            //fine client
        }

        private bool checkConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead("http://clients3.google.com/generate_204"))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}