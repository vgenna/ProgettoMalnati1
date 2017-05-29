using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace ProgettoMalnati1
{
    class Client
    {
        public Client()
        {

        }

        public void startBroadcast()
        {
            UdpClient newClient = new UdpClient(); //client per inviare il messaggio broadcast
            byte[] requestData = Encoding.ASCII.GetBytes("Request Data"); //dati da inviare in broadcast
            IPEndPoint remoteServer = new IPEndPoint(IPAddress.Any, 0); //server da cui ricevere le risposte

            newClient.EnableBroadcast = true;
            newClient.Send(requestData, requestData.Length, new IPEndPoint(IPAddress.Broadcast,1500));


            byte[] serverResponseBytes = newClient.Receive(ref remoteServer);
            string serverResponse = Encoding.ASCII.GetString(serverResponseBytes);

            string formattedString = string.Format("Client: Recieved '{0}' from {1}", serverResponse, remoteServer.Address.ToString());

            MessageBox.Show(formattedString);

            newClient.Close();
        }
    }
}
