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
using System.Windows.Shapes;

namespace ProgettoMalnati1
{
    /// <summary>
    /// Logica di interazione per ImpostazioniPrimoAvvio.xaml
    /// </summary>
    public partial class ImpostazioniPrimoAvvio : Window
    {
        //Server s;
        public ImpostazioniPrimoAvvio()
        {
            InitializeComponent();
            //s = myServer;
        }

      
        private void button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("cliccato");  
            if(pubbl.IsChecked == true)
            {
                //s.privato = false;
            }
            else if(priv.IsChecked == true)
            {
                //s.privato = true;
            }
            //this.Close();

            Server s = new Server(2);
            
            //s.oSignalEvent.Set();
        }

       
    }
}
