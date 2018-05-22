using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        Server s;
        bool privato;
        string selectedPath = AppDomain.CurrentDomain.BaseDirectory; //default
        Uri image = new Uri("pack://application:,,,/Resource/ProfileImages/download.jpg");

        public ImpostazioniPrimoAvvio()
        {
            InitializeComponent();
            pubbl.IsChecked = true;
            textBlock.Width = selectedPath.Length*6;
            textBlock.Text = selectedPath;
            profileImage.Source = new BitmapImage(image);
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

                s = new Server(privato, selectedPath, nome, conferma, image); 

                //s.oSignalEvent.Set();
            }
            catch(Exception ex)
            {
                //gestire l'eccezione
                System.Windows.MessageBox.Show(ex.Message + "\n");
            }
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            dialog.Description = "Scegli percorso in cui salvare il file: ";
            DialogResult result = dialog.ShowDialog();
            if (result.ToString()=="OK") {
                selectedPath = dialog.SelectedPath;
                textBlock.Width = selectedPath.Length * 6;
                textBlock.Text = selectedPath;
            }
      
        }

        private void changeImage_Click(object sender, RoutedEventArgs e)
        {
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
                profileImage.Source = new BitmapImage(image);
            }
        }
    }
}
