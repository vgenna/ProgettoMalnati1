﻿using System;
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
    public class Server
    {
        public Server()
        {

        }

        public void startBroadcastSocket()
        {
            UdpClient listener = new UdpClient(1500); //LA PORTA VA SCELTA CASUALMENTE E INVIATA AL CLIENT
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
            sending_socket.SendTo(Encoding.ASCII.GetBytes("NINOzzp"), sending_end_point);
            MessageBox.Show("Server - Sent!");
            sending_socket.Close();
            listener.Close();
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
                        string SaveFileName = "provaRic.txt";//string.Empty;
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
                                //string s = string.Format("Ricevuti {0} byte", totalrecbytes);
                                //MessageBox.Show(s);
                            }
                            Fs.Close();
                            MessageBox.Show("File ricevuto!");

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
                catch (Exception e) { MessageBox.Show(e.Message); }

            }


        }


    }

}


