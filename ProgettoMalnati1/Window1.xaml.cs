using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
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
    /// Logica di interazione per Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        TcpListener listener; 

        public Window1()
        {
            InitializeComponent();
        }

        public Window1(TcpListener Listener)
        {
            InitializeComponent();
            this.Visibility = Visibility.Visible;
            this.listener = Listener;
        }

        private void mant_Click(object sender, RoutedEventArgs e)
        {
            //non faccio niente
            //this.Close();
        }

        private void cambiaImp_Click(object sender, RoutedEventArgs e)
        {
            //faccio il riavvio dell'applicazione
            //this.Close();
            this.listener.Server.Close();
            System.Windows.Forms.Application.Restart();
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }
    }
}
