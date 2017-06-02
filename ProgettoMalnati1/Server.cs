﻿using System;
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

        public void receiveString(int portN)
        {

            TcpListener Listener = null;
            try
            {
                Listener = new TcpListener(IPAddress.Any, portN);
                Listener.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            int BufferSize = 1024;

            byte[] RecData = new byte[BufferSize];

            for(; ;)
            {
                TcpClient client = null;
                NetworkStream netstream = null;

                try
                {
                    if(Listener.Pending())
                    {
                        client = Listener.AcceptTcpClient();
                        netstream = client.GetStream();

                        int r = netstream.Read(RecData, 0, RecData.Length);
                        /**messaggio in RecData**/
                        string s = string.Format("Stringa ricevuta -> {0}",RecData);
                        MessageBox.Show(s);
                        netstream.Close();
                        client.Close();
                    }
                }
                catch(Exception e) { Console.WriteLine(e.Message);  }

            }


        }
    }
}
