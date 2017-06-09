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
using System;
using System.Drawing;
//
using System.Windows.Forms;

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
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
           
            if (clientButton.IsChecked == true)
            {
                Client c = new Client();
                //c.startBroadcast();
                //c.sendString("127.0.0.1", 1500, "frosinone culone");
                c.sendFileTCP("127.0.0.1",1500,"mav");
            }
            else
            {
                if (serverButton.IsChecked == true)
                {
                    Server s = new Server();
                    //s.startBroadcast();
                    //s.receiveString(1500);
                    s.receiveFileTCP(1500);
                }
            }
        }




    }
}
