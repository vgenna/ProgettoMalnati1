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
/*********/

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;

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
                if (Fs == null)
                {
                    string s = string.Format("File non trovato.");
                    MessageBox.Show(s);
                    return;
                }
                else
                {
                    string s = string.Format("File trovato.");
                    MessageBox.Show(s);
                }
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
                    //progress_bar.PerformStep();

                }


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

    }
    
}

