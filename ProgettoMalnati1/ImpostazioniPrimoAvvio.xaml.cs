using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ProgettoMalnati1
{
    /// <summary>
    /// Logica di interazione per ImpostazioniPrimoAvvio.xaml
    /// </summary>
    public partial class ImpostazioniPrimoAvvio : Window
    {
        Server s; //vedi riga 107
        bool privato;
        string selectedPath = AppDomain.CurrentDomain.BaseDirectory; //default
        Uri image = new Uri("pack://application:,,,/Resource/ProfileImages/download.jpg");
        bool inizio = true;


        public ImpostazioniPrimoAvvio()
        {
            //if(inizio == false)
            //System.Windows.Application.Current.Shutdown();
            InitializeComponent();
            /***/

            /**finestra riempita con i parametri del file di configurazione**/
            if (ConfigurationManager.AppSettings.Get("pubblico").Equals("true"))
                pubbl.IsChecked = true;
            else
                priv.IsChecked = true;
            textBlock.Width = selectedPath.Length * 6;
            if (ConfigurationManager.AppSettings.Get("selectedPath").Equals(""))
                textBlock.Text = AppDomain.CurrentDomain.BaseDirectory; //textBlock.Text = selectedPath;
            else
                textBlock.Text = ConfigurationManager.AppSettings.Get("selectedPath");
            Uri u = new Uri(ConfigurationManager.AppSettings.Get("image"));
            profileImage.Source = new BitmapImage(u);//profileImage.Source = new BitmapImage(image);
            nomeUtente.Text = ConfigurationManager.AppSettings.Get("nome");
            if (ConfigurationManager.AppSettings.Get("conferma").Equals("true"))
                conf.IsChecked = true;
            else
                conf.IsChecked = false;
            /**/

            privato = false;

            /**senza reg file, modifico la chiave di registro prima per i file e poi per le cartelle**/
            RegistryKey key = Registry.ClassesRoot.OpenSubKey("*\\shell", true);
            key.CreateSubKey("AppMalnati");
            key = key.OpenSubKey("AppMalnati", true);

            key.CreateSubKey("command");
            key = key.OpenSubKey("command", true);

            /**il primo valore e' "" così da settare il valore predefinito**/
            key.SetValue("", "\"" + AppDomain.CurrentDomain.BaseDirectory + "ProgettoMalnati1.exe\" \"%1\" \"client\"");



            key = Registry.ClassesRoot.OpenSubKey("Directory\\shell", true);
            key.CreateSubKey("AppMalnati");
            key = key.OpenSubKey("AppMalnati", true);
            key.CreateSubKey("command");
            key = key.OpenSubKey("command", true);
            key.SetValue("", "\"" + AppDomain.CurrentDomain.BaseDirectory + "ProgettoMalnati1.exe\" \"%1\" \"client\"");
            /*usando il file .reg
            Process regeditProcess = Process.Start("regedit.exe", "/s o.reg");
            regeditProcess.WaitForExit();*/

            //s = myServer;

        }



        private void button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (checkConnection() == false)
                {
                    System.Windows.Forms.MessageBox.Show("Connessione internet assente", "Errore connessione internet", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                //if (inizio == true)
                //{

                

                if (pubbl.IsChecked == true)
                {
                    privato = false;
                }
                else if (priv.IsChecked == true)
                {
                    privato = true;
                }
                //this.Close();

                //leggo nome utente
                string nome = nomeUtente.Text;
                string[] words = System.Text.RegularExpressions.Regex.Split(nome, @"\s+");

                if (words.Length == 0 || words[0] == "")
                {
                    //terminiamo il processo ???????????????????? -> concordare con Enzo
                    System.Windows.Forms.MessageBox.Show("INSERISCI NOME UTENTE.");
                    return;
                }
                nome = null;
                for (int i = 0; i < words.Length; i++)
                {
                    nome = nome + words[i];
                }

                //controllo che sia stata spuntata la casella di conferma ricezione
                bool conferma = false;
                if (conf.IsChecked == true)
                {
                    conferma = true;
                }
                else
                    conferma = false;

                this.Close();


                /*Server*/
                s = new Server(privato, selectedPath, nome, conferma, image);

                //s.oSignalEvent.Set();

                //inizio = false;

                //}
                /*else
                {
                    //System.Windows.Application.Current.Shutdown();
                    if (pubbl.IsChecked == true)
                    {
                        privato = false;
                    }
                    else if (priv.IsChecked == true)
                    {
                        privato = true;
                    }
                    //this.Close();

                    //leggo nome utente
                    string nome = nomeUtente.Text;
                    string[] words = System.Text.RegularExpressions.Regex.Split(nome, @"\s+");

                    if (words.Length == 0 || words[0] == "")
                    {
                        //terminiamo il processo ???????????????????? -> concordare con Enzo
                        System.Windows.Forms.MessageBox.Show("INSERISCI NOME UTENTE.");
                        return;
                    }
                    nome = null;
                    for (int i = 0; i < words.Length; i++)
                    {
                        nome = nome + words[i];
                    }

                    //controllo che sia stata spuntata la casella di conferma ricezione
                    bool conferma = false;
                    if (conf.IsChecked == true)
                    {
                        conferma = true;
                    }
                    else
                        conferma = false;

                    this.Close();


                    //Server
                    s = new Server(privato, selectedPath, nome, conferma, image);

                }*/
            }
            catch (Exception ex)
            {
                //gestire l'eccezione
                System.Windows.MessageBox.Show(ex.Message + "\n");
            }
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

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            dialog.Description = "Scegli percorso in cui salvare il file: ";
            DialogResult result = dialog.ShowDialog();
            if (result.ToString() == "OK")
            {
                selectedPath = dialog.SelectedPath;
                textBlock.Width = selectedPath.Length * 6;
                textBlock.Text = selectedPath;
            }

        }

        private void changeImage_Click(object sender, RoutedEventArgs e)
        {
            Socket sending_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Title = "Scegli un'immagine da usare come foto profilo";
            dlg.InitialDirectory = System.IO.Path.Combine(System.IO.Path.GetFullPath(@"..\..\"), "Resource\\ProfileImages");
            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".png";
            dlg.Filter = "All files (*.*)|*.*|PNG Files (*.png)|*.png|JPEG Files (*.jpeg)|*.jpeg|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                image = new Uri(dlg.FileName);
                /*controllo dimensione immagine selezionata*/
                var imageToSend = System.Drawing.Image.FromFile(image.LocalPath);
                MemoryStream ms = new MemoryStream();
                imageToSend.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                var msA = ms.ToArray();
                if (msA.Length > 65536)
                {
                    System.Windows.Forms.MessageBox.Show("Non sono consentite immagini del profilo di qualita' superiore a 8KB", "Errore immagine profilo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                /**/
                else
                    profileImage.Source = new BitmapImage(image);
            }
        }
    }
}
