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
            try
            {
                UdpClient udpClient;
                IPAddress bindAddress = IPAddress.Parse("192.168.1.5");
                IPAddress groupListenAddress = IPAddress.Parse("224.5.6.7");
                int port = 2222;


                udpClient = new UdpClient();
                udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                udpClient.Client.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastLoopback, true);

                IPEndPoint localEndPoint = new IPEndPoint(bindAddress, port);
                udpClient.Client.Bind(localEndPoint);

                udpClient.JoinMulticastGroup(groupListenAddress);

                IPEndPoint otherEP = null; //verrà popolato al Receive()
                string received_data;
                byte[] receive_byte_array;

                receive_byte_array = udpClient.Receive(ref otherEP);
                received_data = Encoding.ASCII.GetString(receive_byte_array, 0, receive_byte_array.Length);
                string formattedString = string.Format("Client - Received from {0} , Received data: {1}", otherEP.ToString(), received_data);
                MessageBox.Show(formattedString);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

        }
        public void receiveFileTCP(int portN)
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
            int BufferSize = 1024*1024;

            byte[] RecData = new byte[BufferSize];
            int RecBytes;
            for (;;)
            {
                TcpClient client = null;
                NetworkStream netstream = null;
                string Status = string.Empty;

                try
                {
                    //string message = "Accept the Incoming File ";
                    //string caption = "Incoming Connection";
                    // MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                    //DialogResult result;


                    if (Listener.Pending())
                    {
                        client = Listener.AcceptTcpClient();
                        netstream = client.GetStream();
                        Status = "Connected to a client\n";
                        //result = System.Windows.MessageBox.Show(message, caption, buttons);

                        //file selection
                        string SaveFileName = "ricevuto1.pdf";//string.Empty;
                        /*SaveFileDialog DialogSave = new SaveFileDialog();
                        DialogSave.Filter = "All files (*.*)|*.*";
                        DialogSave.RestoreDirectory = true;
                        DialogSave.Title = "Where do you want to save the file?";
                        DialogSave.InitialDirectory = @"C:/";
                        if (DialogSave.ShowDialog() == DialogResult.OK)
                            SaveFileName = DialogSave.FileName;*/
                        if (SaveFileName != string.Empty)
                        {
                            int totalrecbytes = 0;
                            FileStream Fs = new FileStream(SaveFileName, FileMode.OpenOrCreate, FileAccess.Write);
                            while ((RecBytes = netstream.Read(RecData, 0, RecData.Length)) > 0)
                            {
                                Fs.Write(RecData, 0, RecBytes);
                                totalrecbytes += RecBytes;
                                string s = string.Format("Ricevuti {0} byte", totalrecbytes);
                                MessageBox.Show(s);
                            }
                            Fs.Close();

                        }
                        else
                        {
                            string s = string.Format("File non trovato.");
                            System.Windows.MessageBox.Show(s);
                        }


                        netstream.Close();
                        client.Close();
                    }
                }
                catch (Exception e) { MessageBox.Show("Eccezione!!"); }

            }


        }


    }

}


