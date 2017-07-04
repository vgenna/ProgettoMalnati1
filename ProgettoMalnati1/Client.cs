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
using ProgettoMalnati1;
using System.Windows.Controls;

using NetworkCommsDotNet;
using NetworkCommsDotNet.Connections.UDP;
using NetworkCommsDotNet.Connections;

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
            newClient.Client.ReceiveTimeout = 3000;

            newClient.EnableBroadcast = true;
            newClient.Send(requestData, requestData.Length, new IPEndPoint(IPAddress.Broadcast, 1500));


            while (true)
            {
                try
                {
                    byte[] serverResponseBytes = newClient.Receive(ref remoteServer);

                    string serverResponse = Encoding.ASCII.GetString(serverResponseBytes);

                    string formattedString = string.Format("Client: Received '{0}' from {1}", serverResponse, remoteServer.Address.ToString());

                    MessageBox.Show(formattedString);
                }
                catch (SocketException e)
                {
                    MessageBox.Show(e.Message + " - Timeout expired!");
                    break;
                }
                catch (Exception e)
                {
                    MessageBox.Show("Exception: " + e.Message);
                    break;
                }
            }
        }

        public void startBroadcastSocket()
        {

            // ipAddress = Dns.Resolve(Dns.GetHostName()).AddressList[0]; RECUPERARE PROPRIO INDIRIZZO IP
            try
            {



                //Socket sending_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                //sending_socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
                //sending_socket.Bind(new IPEndPoint(IPAddress.Parse("172.21.51.15"), 1502));

                IPEndPoint listenEndPoint = new IPEndPoint(IPAddress.Parse("192.168.1.6"), 1501);
                UdpClient client = new UdpClient(listenEndPoint);
                client.EnableBroadcast = true;


                MessageBox.Show("Trying to send");

                IPEndPoint groupEP = new IPEndPoint(IPAddress.Parse("192.168.1.255"), 1500);
                IPAddress send_to_address = groupEP.Address; //togliere groupEP e lasciare IP.Broadcast solo (o solo new IP(Broadcast, 1500))
                IPEndPoint sending_end_point = new IPEndPoint(send_to_address, 1500);

                //sending_socket.Connect(sending_end_point);
                //sending_socket.SendTo(Encoding.ASCII.GetBytes("Request Data"), sending_end_point);

                byte[] requestData = Encoding.ASCII.GetBytes("Request Data"); //dati da inviare in broadcast
                client.Send(requestData, requestData.Length, sending_end_point);

                MessageBox.Show("Client - Sent!");


                //IPEndPoint listenEndPoint = new IPEndPoint(IPAddress.Parse("172.21.51.15"), 1501);
                //UdpClient listener = new UdpClient(listenEndPoint);
                IPEndPoint otherEP = null; //verrà popolato al Receive()
                string received_data;
                byte[] receive_byte_array;

                receive_byte_array = client.Receive(ref otherEP);
                received_data = Encoding.ASCII.GetString(receive_byte_array, 0, receive_byte_array.Length);
                string formattedString = string.Format("Client - Received from {0} , Received data: {1}", otherEP.ToString(), received_data);
                MessageBox.Show(formattedString);

                client.Close();

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public void sendFileTCP(string IP_addr, Int32 port_number, string nome_file)
        {
            /**************
            cancellare la classe progressbar già implementata in windows
            **********/
            TcpClient client = null;
            NetworkStream netstream = null;
            try
            {
                client = new TcpClient(IP_addr, port_number);
                netstream = client.GetStream();
                int BufferSize = 1024;/**dimensione del pacchetto inviato**/

                FileStream Fs = new FileStream(nome_file, FileMode.Open, FileAccess.Read);/**apro il file in lettura**/
                int packets_number = Convert.ToInt32
         (Math.Ceiling(Convert.ToDouble(Fs.Length) / Convert.ToDouble(BufferSize)));
                /**il numero di pacchetti da inviare è pari alla grandezza totale del file diviso la 
            grandezza del buffer**/

                ProgressBar progress_bar = new ProgressBar();
                progress_bar.Value = 0;
                progress_bar.Minimum = 0;
                progress_bar.Maximum = packets_number;
                //progress_bar.

                int lunghezza_file = (int)Fs.Length, dim_pacchetto_corrente, cont = 0;

                for (int i = 0; i < packets_number; i++)
                {
                    if (lunghezza_file > BufferSize)
                    {
                        dim_pacchetto_corrente = BufferSize;
                        lunghezza_file = lunghezza_file - dim_pacchetto_corrente;
                    }
                    else
                        dim_pacchetto_corrente = lunghezza_file;
                    byte[] SendingBuffer = new byte[dim_pacchetto_corrente];
                    Fs.Read(SendingBuffer, 0, dim_pacchetto_corrente);
                    netstream.Write(SendingBuffer, 0, (int)SendingBuffer.Length);
                    if (progress_bar.Value >= progress_bar.Maximum)
                        progress_bar.Value = progress_bar.Minimum;
                    //!!!!!!!!!!!!!!!!!!!!!completare PROGRESSBAR
                    ///////////////progress_bar.PerformStep();

                }


            }
            catch (Exception e)

            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                netstream.Close();
                client.Close();
            }
        }


        public void sendString(string IP_addr, Int32 port_number, string s)
        {

            TcpClient client = null;
            NetworkStream netstream = null;
            try
            {
                client = new TcpClient(IP_addr, port_number);
                netstream = client.GetStream();
                byte[] bytes = Encoding.ASCII.GetBytes(s);
                netstream.Write(bytes, 0, (int)bytes.Length);
                MessageBox.Show("Client - Sent!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                netstream.Close();
                client.Close();
            }


        }

        public void startBroadcastNetworkComms()
        {

            MessageBox.Show("Trying to send");

            UDPConnection.SendObject("ChatMessage", "This is the broadcast test message!", new IPEndPoint(IPAddress.Broadcast, 1501));

            MessageBox.Show("Client - Sent!");

            //We need to define what happens when packets are received.
            //To do this we add an incoming packet handler for a 'ChatMessage' packet type. 
            //
            //We will define what we want the handler to do inline by using a lambda expression
            //http://msdn.microsoft.com/en-us/library/bb397687.aspx.
            //We could also just point the AppendGlobalIncomingPacketHandler method 
            //to a standard method (See AdvancedSend example)
            //
            //This handler will convert the incoming raw bytes into a string (this is what 
            //the <string> bit means) and then write that string to the local console window.
            NetworkComms.AppendGlobalIncomingPacketHandler<string>("ChatMessage",
                (packetHeader, connection, incomingString) =>
                {
                    string formattedString = string.Format("Client - Received from {0} , Received data: {1}", connection.ToString(), incomingString);
                    MessageBox.Show(formattedString);
                });

            //Start listening for incoming UDP data
            Connection.StartListening(ConnectionType.UDP, new IPEndPoint(IPAddress.Any, 1500));
        }
        public void clientMulticast()
        {
            try
            {
                UdpClient udpClient;
                IPAddress bindAddress = IPAddress.Parse("192.168.1.8");
                IPAddress groupListenAddress = IPAddress.Parse("224.5.6.7");
                int port = 2222;


                udpClient = new UdpClient();
                udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                udpClient.Client.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastLoopback, true);

                IPEndPoint localEndPoint = new IPEndPoint(bindAddress, port);
                udpClient.Client.Bind(localEndPoint);

                udpClient.JoinMulticastGroup(groupListenAddress);

                IPEndPoint remoteep = new IPEndPoint(groupListenAddress, 2222);

                MessageBox.Show("Trying to send");

                //sending_socket.Connect(sending_end_point);
                //sending_socket.SendTo(Encoding.ASCII.GetBytes("Request Data"), sending_end_point);

                byte[] requestData = Encoding.ASCII.GetBytes("Request Data"); //dati da inviare in broadcast
                udpClient.Send(requestData, requestData.Length, remoteep);

                MessageBox.Show("Client - Sent!");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

        }
    }
}

