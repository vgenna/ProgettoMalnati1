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
                try
                {
                    IPEndPoint remoteClient = new IPEndPoint(IPAddress.Any, 0);
                    byte[] clientRequestBytes = newServer.Receive(ref remoteClient);
                    string clientRequest = Encoding.ASCII.GetString(clientRequestBytes);

                    string formattedString = string.Format("Server: Received '{0}' from {1}, sending response", clientRequest, remoteClient.Address.ToString());

                    MessageBox.Show(formattedString);
                    newServer.Send(responseData, responseData.Length, remoteClient);
                    newServer.Send(Encoding.ASCII.GetBytes("Other Response Data"), responseData.Length, remoteClient);
                }
                catch (Exception e)
                {
                    MessageBox.Show("Exception: " + e.Message);
                }
                finally
                {
                    // newServer.Close();
                }
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

            for (;;)
            {
                TcpClient client = null;
                NetworkStream netstream = null;

                try
                {
                    if (Listener.Pending())
                    {
                        client = Listener.AcceptTcpClient();
                        netstream = client.GetStream();

                        int r = netstream.Read(RecData, 0, RecData.Length);
                        /**messaggio in RecData**/
                        string s2 = Encoding.ASCII.GetString(RecData);
                        string s = string.Format("Stringa ricevuta -> {0}", s2);
                        MessageBox.Show(s);
                        netstream.Close();
                        client.Close();
                    }
                }
                catch (Exception e) { Console.WriteLine(e.Message); }

            }


        }



    }



}

