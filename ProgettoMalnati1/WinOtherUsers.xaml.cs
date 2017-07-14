using System;
using System.Collections.Generic;
using System.Linq;
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

            //qui devo selezionare quelli a cui voglio inviare e devo metterli in una nuova lista (usersToShare)
            List<OtherUser> usersToShare = new List<OtherUser>();
            foreach (UIElement child in stackP.Children)
            {
                if (child is CheckBox)
                {
                    var childCB = child as CheckBox;
                    if (childCB.IsChecked == true)
                    {
                        usersToShare.Add(c.otherUsers[childCB.Name]);
                    }
                }
            }


            string nomefile = "prova.txt";
            List<Thread> threadList = new List<Thread>();
            foreach (OtherUser ou in usersToShare)
            {
                Thread newthread = new Thread(() => c.sendFileTCP(ou.Address.ToString(), 1500, nomefile));
                newthread.SetApartmentState(ApartmentState.STA);
                threadList.Add(newthread);
                newthread.Start();
                //Application.Current.Dispatcher.BeginInvoke((Action)delegate {
                //UTILIZZARE BACKGROUND WORKER PER NON FAR BLOCCARE L'INTERFACCIA
                //    c.sendFileTCP(ou.Address.ToString(), 1500, nomefile);

                //});
            }

            this.Close();

            foreach(Thread t in threadList)
            {
                t.Join();
            }

            MessageBox.Show(string.Format("File {0} condiviso!", nomefile));
        }
    }
}
