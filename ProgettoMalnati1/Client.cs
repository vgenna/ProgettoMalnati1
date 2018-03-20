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
using System.Windows.Threading;

namespace ProgettoMalnati1
{
    public class Client
    {
        public Dictionary<string, OtherUser> otherUsers;
        public List<OtherUser> usersToShare;
        public string filename;
        public long nBytesTot;

        public Client(string s)
        {
            otherUsers = new Dictionary<string, OtherUser>();
            usersToShare = new List<OtherUser>();
            filename = s;
        }

        public void startBroadcastSocket()
        {
            string myIP = GetLocalIPAddress(); 
            //string myIP = "127.0.0.1"; //USARE QUESTO PER PROVARE IN LOCALE QUANDO SI è TIPO SU EDUROAM
            
            IPEndPoint listenEndPoint = new IPEndPoint(IPAddress.Parse(myIP), 1501);
            UdpClient client = new UdpClient(listenEndPoint);
            client.EnableBroadcast = true;
            client.Client.ReceiveTimeout = 2000;
            bool timeout = false;
            try
            {
                MessageBox.Show("Trying to send");
                string broadcastAddress = getBroadcastAdd(myIP);
                //MessageBox.Show(broadcastAddress);
                IPEndPoint sending_end_point = new IPEndPoint(IPAddress.Parse(broadcastAddress), 1500); 

                byte[] requestData = Encoding.ASCII.GetBytes("Request Data Malnati"); //dati da inviare in broadcast-->NOME FILE-->NO! VA NELL'INVIO TCP

                //for(int i=0;i<5;i++)
                 client.Send(requestData, requestData.Length, sending_end_point); //VEDERE SE IL PROBLEMA ERA SOLO IL PC DI SAVERIO E NE BASTA 1

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
                    }
                    catch (SocketException)
                    {
                        //MessageBox.Show("Timeout expired!");
                        timeout = true;
                    }
                }
                //qui dovrei avere la lista con tutti i vari (IP,nome) di quelli che hanno risposto al messaggio udp entro 2 secondi

                client.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show("Exception: " + e.Message);
                client.Close();
            }
        }

        private string getBroadcastAdd(string myIP)
        {
            int lenght = myIP.Length;
            Char delimiter = '.';
            string[] fields = myIP.Split(delimiter);
            string broadcast = fields[0] + "." + fields[1] + "." + fields[2] + ".255";

            return broadcast;
        }

        public void sendFileTCP(string IP_addr, Int32 port_number, string nome_file, ProgressBar pBar)
        {
            TcpClient client = null;
            NetworkStream netstream = null;
            try
            {
                client = new TcpClient(IP_addr, port_number);
                netstream = client.GetStream();
                int BufferSize = 1024;/**dimensione del pacchetto inviato**/

                FileStream Fs = new FileStream(nome_file, FileMode.Open, FileAccess.Read);/**apro il file in lettura**/
                int packets_number = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(Fs.Length) / Convert.ToDouble(BufferSize)));
                /**il numero di pacchetti da inviare è pari alla grandezza totale del file diviso la 
            grandezza del buffer**/

                int lunghezza_file = (int)Fs.Length, dim_pacchetto_corrente;

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

                    //aggiorno la progressBar con il numero dei byte inviati
                    //pBar.Dispatcher.BeginInvoke((Action)delegate { pBar.Value += (dim_pacchetto_corrente / nBytesTot) * 100; }); // Invoke esegue sullo stesso thread (sincrono)
                    pBar.Value += (dim_pacchetto_corrente / nBytesTot) * 100;
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

   private static string GetLocalIPAddress() {
            string myIP = null;
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 /*|| ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet*/)
                {
                    foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            myIP = ip.Address.ToString();
                        }
                    }
                }
            }
            if (myIP != null)
                return myIP;
            else
                throw new Exception("Local IP Address Not Found!");
        }
    }
}

