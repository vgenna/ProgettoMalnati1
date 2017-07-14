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
using System.Net.NetworkInformation;

namespace ProgettoMalnati1
{
    public class Client
    {
        public Dictionary<string, OtherUser> otherUsers;

        public Client()
        {
            otherUsers = new Dictionary<string, OtherUser>();
        }

        public void startBroadcastSocket()
        {
            string myIP = GetLocalIPAddress();
            //MessageBox.Show(myIP);
            IPEndPoint listenEndPoint = new IPEndPoint(IPAddress.Parse(myIP), 1501);
            UdpClient client = new UdpClient(listenEndPoint);
            client.EnableBroadcast = true;
            client.Client.ReceiveTimeout = 2000;
            bool timeout = false;
            try
            {
                MessageBox.Show("Trying to send");

                IPEndPoint sending_end_point = new IPEndPoint(IPAddress.Parse("192.168.1.255"), 1500);

                byte[] requestData = Encoding.ASCII.GetBytes("Request Data Malnati"); //dati da inviare in broadcast

                for(int i=0;i<5;i++)
                 client.Send(requestData, requestData.Length, sending_end_point);

                MessageBox.Show("Client - Sent!");

                while (!timeout)
                { //esce da questo ciclo solo in caso di timeout o di eccezione
                    try
                    {
                        IPEndPoint otherEP = null; //verrà popolato al Receive()
                        string received_data;
                        byte[] receive_byte_array;

                        receive_byte_array = client.Receive(ref otherEP);
                        received_data = Encoding.ASCII.GetString(receive_byte_array, 0, receive_byte_array.Length);
                        
                        otherUsers.Add(received_data, new OtherUser(otherEP.Address, received_data)); //GESTIRE CASO DI CHIAVE UGUALE
                        
                        //string formattedString = string.Format("Client - Received from {0} , Received data: {1}", otherEP.Address, received_data);
                        //MessageBox.Show(formattedString);
                    }
                    catch (SocketException e)
                    {
                        //MessageBox.Show("Timeout expired!");
                        timeout = true;
                    }
                }
                //qui dovrei avere la lista con tutti i vari (IP,nome) di quelli che hanno risposto al messaggio udp entro 2 secondi

                //TOGLIEREEEEEEEEEEEEEEE
                foreach(OtherUser ou in otherUsers.Values)
                {
                    string formattedString = string.Format("{0} is at IP: {1}", ou.Name, ou.Address);
                   // MessageBox.Show(formattedString);
                }
                client.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show("Exception: " + e.Message);
                client.Close();
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

   public static string GetLocalIPAddress() {
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 /*|| ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet*/)
                {
                    foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            return ip.Address.ToString();
                        }
                    }
                }
            }
            throw new Exception("Local IP Address Not Found!");
        }
    }
}

