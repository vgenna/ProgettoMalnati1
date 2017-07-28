using System;
using System.Collections.Generic;
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
        //Server s;
        bool privato;
        public ImpostazioniPrimoAvvio()
        {
            InitializeComponent();
            privato = false;
            //s = myServer;
        }

      
        private void button_Click(object sender, RoutedEventArgs e)
        { 
            if(pubbl.IsChecked == true)
            {
                privato = false;
            }
            else if(priv.IsChecked == true)
            {
                privato = true;
            }

            this.Close();

            string selectedPath = null;
            var dialog = new FolderBrowserDialog();
            dialog.Description = "Scegli percorso in cui salvare i file: ";
            DialogResult result = dialog.ShowDialog();
            selectedPath = dialog.SelectedPath;

            //System.Windows.MessageBox.Show("Il percorso è: "+selectedPath);

            Server s = new Server(privato, selectedPath); //forse nel costruttore conviene inizializzare solo i campi e poi fare un metodo tipo s.Start() per fare eseguire tutto il codice del server
        }

       
    }
}
