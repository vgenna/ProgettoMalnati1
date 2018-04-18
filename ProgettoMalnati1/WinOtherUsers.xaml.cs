using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ProgettoMalnati1
{
    /// <summary>
    /// Interaction logic for WinOtherUsers.xaml
    /// </summary>
    public partial class WinOtherUsers : Window
    {
        Client c;
        public WinOtherUsers()
        {
            InitializeComponent();
        }

        public WinOtherUsers(Client newC)
        {
            InitializeComponent();
            c = newC;
        }

        private void condividiButton_Click(object sender, RoutedEventArgs e)
        {
            int nUtenti = 0;
            //qui devo selezionare quelli a cui voglio inviare e devo metterli nella lista (usersToShare)
            foreach (UIElement child in stackP.Children)
            {
                if (child is CheckBox)
                {
                    var childCB = child as CheckBox;
                    if (childCB.IsChecked == true)
                    {
                        c.usersToShare.Add(c.otherUsers[childCB.Name]);
                        nUtenti++;
                    }
                }
            }

            if (nUtenti>0)
            {
                WinProgBar winPB = new WinProgBar(c);
                this.Close();
            }
            else
            {
                MessageBox.Show("Asino seleziona un utente");
            }        
        }

    }
}
