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
    class Server
    {
        public Server()
        {

        }

        public void startBroadcast()
        {
            UdpClient newServer = new UdpClient(1500);
            byte[] responseData = Encoding.ASCII.GetBytes("Response Data");

            while (true)
            {
                IPEndPoint remoteClient = new IPEndPoint(IPAddress.Any, 0);
                byte[] clientRequestBytes = newServer.Receive(ref remoteClient);
                string clientRequest = Encoding.ASCII.GetString(clientRequestBytes);

                string formattedString = string.Format("Server: Recieved '{0}' from {1}, sending response", clientRequest, remoteClient.Address.ToString());

                MessageBox.Show(formattedString);
                newServer.Send(responseData, responseData.Length, remoteClient);
            }
        }
    }
}
