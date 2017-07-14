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
                c.startBroadcastSocket();
                WinOtherUsers winOU = new WinOtherUsers(c);

                

                foreach (OtherUser ou in c.otherUsers.Values)
                {
                    //string formattedString = string.Format("{0} is at IP: {1}", ou.Name, ou.Address);
                    //ListBoxItem itm = new ListBoxItem();
                    //itm.Content = formattedString;

                    //MessageBox.Show(c.otherUsers[ou.Name].Address.ToString()); //per prendere gli indirizzi sapendo il nome
                    CheckBox cb = new CheckBox();
                    cb.Name = ou.Name;
                    cb.Content = ou.Name;
                    winOU.stackP.Children.Add(cb);
                    //winOU.listBox.Items.Add(itm);
                    //winOU.listView.Items.Add(formattedString);
                }

                winOU.Show();
            }
            else
            {
                if (serverButton.IsChecked == true)
                {
                    Server s = new Server();
                   
                    s.startBroadcastSocket();
                    
                    s.receiveFileTCP(1500);
                }
            }
        }
    }
}
