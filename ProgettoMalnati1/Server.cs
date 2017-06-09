using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using System.IO;

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
            int BufferSize = 1024;

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
                        string SaveFileName = string.Empty;
                        SaveFileDialog DialogSave = new SaveFileDialog();
                        DialogSave.Filter = "All files (*.*)|*.*";
                        DialogSave.RestoreDirectory = true;
                        DialogSave.Title = "Where do you want to save the file?";
                        DialogSave.InitialDirectory = @"C:/";
                        if (DialogSave.ShowDialog() == DialogResult.OK)
                            SaveFileName = DialogSave.FileName;
                        if (SaveFileName != string.Empty)
                        {
                            int totalrecbytes = 0;
                            FileStream Fs = new FileStream(SaveFileName, FileMode.OpenOrCreate, FileAccess.Write);
                            while ((RecBytes = netstream.Read(RecData, 0, RecData.Length)) > 0)
                            {
                                Fs.Write(RecData, 0, RecBytes);
                                totalrecbytes += RecBytes;
                                string s = string.Format("Ricevuti {0} byte", totalrecbytes);
                                System.Windows.MessageBox.Show(s);
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
                catch (Exception e) { Console.WriteLine(e.Message); }

            }


        }


    }



}

