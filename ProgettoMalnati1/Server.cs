using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;


using NetworkCommsDotNet;
using NetworkCommsDotNet.Connections.UDP;
using NetworkCommsDotNet.Connections;


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

                    System.Windows.MessageBox.Show(formattedString);
                    newServer.Send(responseData, responseData.Length, remoteClient);
                    newServer.Send(Encoding.ASCII.GetBytes("Other Response Data"), responseData.Length, remoteClient);
                }
                catch (Exception e)
                {
                    System.Windows.MessageBox.Show("Exception: " + e.Message);
                }
                finally
                {
                    // newServer.Close();
                }
            }
        }

        public void startBroadcastSocket()
        {
            UdpClient listener = new UdpClient(1500);
            IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, 1500);
            string received_data;
            byte[] receive_byte_array;

            receive_byte_array = listener.Receive(ref groupEP);
            received_data = Encoding.ASCII.GetString(receive_byte_array, 0, receive_byte_array.Length);
            string formattedString = String.Format("Server - Received a broadcast from {0} , Received data: {1}", groupEP.ToString(), received_data);
            MessageBox.Show(formattedString);

            Socket sending_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram,ProtocolType.Udp);
            IPAddress send_to_address = groupEP.Address;
            IPEndPoint sending_end_point = new IPEndPoint(send_to_address, 1501);
            sending_socket.SendTo(Encoding.ASCII.GetBytes("Response Data"), sending_end_point);
            MessageBox.Show("Server - Sent!");
            sending_socket.Close();
            listener.Close();
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
                System.Windows.MessageBox.Show(ex.Message);
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
                        System.Windows.MessageBox.Show(s);
                        netstream.Close();
                        client.Close();
                    }
                }
                catch (Exception e) { Console.WriteLine(e.Message); }

            }


        }

        public void startBroadcastNetwork()
        {
            //ConnectionInfo connInfo = new ConnectionInfo("192.168.0.1", 1501);
            // Connection newUDPConn = UDPConnection.GetConnection(connInfo, UDPOptions.None);

            //ricezione pacchetti udp
            NetworkComms.AppendGlobalIncomingPacketHandler<string>("ChatMessage", (packetHeader, connection, incomingString) =>
            {
                /*Console.WriteLine("\n  ... Incoming message from " +
                connection.ToString() + " saying '" +
                incomingString + "'.");*/
                string formattedString = String.Format("Server - Received a broadcast from {0} , Received data: {1}", connection.ToString(), incomingString);

                //connection.ConnectionInfo.
                MessageBox.Show(formattedString);
                //UDPConnection.SendObject("ChatMessage", "This is the broadcast test message!", new IPEndPoint(IPAddress.Broadcast, 1501));
                // MessageBox.Show("Server sent!");
            });

            //Start listening for incoming UDP data
            Connection.StartListening(ConnectionType.UDP, new IPEndPoint(IPAddress.Any, 1501));

            //invio

        }

        public void startMulticast() {
            //Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            //IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 4567);
            ////EndPoint groupEP = new IPEndPoint(IPAddress.Any, 1500);
            //s.Bind(ipep);

            //IPAddress ip = IPAddress.Parse("224.5.6.7");

            //s.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(ip, IPAddress.Any));

            //byte[] b = new byte[1024];
            ////s.ReceiveFrom(b, ref groupEP);
            //s.Receive(b);
            //string str = Encoding.ASCII.GetString(b, 0, b.Length);

            //IPAddress address = ((IPEndPoint)s.RemoteEndPoint).Address;

            //string formattedString = string.Format("Server - Received multicast from {0} , Received data: '{1}'", address, str);

            //MessageBox.Show(formattedString);

            //s.Close();

            UdpClient client = new UdpClient();

            client.ExclusiveAddressUse = false;
            IPEndPoint localEp = new IPEndPoint(IPAddress.Any, 2222);

            client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            client.ExclusiveAddressUse = false;

            client.Client.Bind(localEp);

            IPAddress multicastaddress = IPAddress.Parse("239.0.0.222");
            client.JoinMulticastGroup(multicastaddress);

            Console.WriteLine("Listening this will never quit so you will need to ctrl-c it");

            while (true)
            {
                Byte[] data = client.Receive(ref localEp);
                string strData = Encoding.Unicode.GetString(data);
                Console.WriteLine(strData);
            }
        }
    }
}

