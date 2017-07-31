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
using System.Windows.Forms;

namespace ProgettoMalnati1
{
    public class Server
    {
        public bool privato, conferma;//usata nella funzione statica del primo thread
        public string savePath, nomeUtente;
        //public ManualResetEvent oSignalEvent;

        public Server(bool privato, string savePath, string nomeUtente, bool conferma)
        {
            this.privato = privato;//posso annunciarmi sulla rete come server
            this.savePath = savePath;
            this.nomeUtente = nomeUtente;
            this.conferma = conferma;
            Thread thread_server = new Thread(/*Server.startBroadcastSocketStatic*/() => startBroadcastSocketStatic(this.privato, this.nomeUtente));
            thread_server.Start();//ricezione e invio pacchetti UDP
            TcpListener Listener = null;
            try
            {
                Listener = new TcpListener(IPAddress.Any, 1500);
                Listener.Start();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }


            for (;;)
            {
                TcpClient client = null;

                try
                {
                    if (Listener.Pending())
                    {
                        client = Listener.AcceptTcpClient();

                        //string message = "Accept the Incoming File ";
                        //string caption = "Incoming Connection";
                        //MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                        //DialogResult result;
                        //result = MessageBox.Show(message, caption, buttons);
                        // if (result == System.Windows.Forms.DialogResult.Yes) {}



                        //thread dopo ogni accept
                        Thread thread_invio = new Thread(() => receiveNewFile(client, this.savePath, this.conferma));
                        thread_invio.Start();
                    }
                }
                catch (Exception e) { Console.WriteLine(e.Message); }

            }


        }

        //prima funzione del thread
        public static void startBroadcastSocketStatic(bool pr, string nomeUtente)
        {
            UdpClient listener = new UdpClient(1500);
            IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, 1500);
            string received_data;
            byte[] receive_byte_array;


            while (true)
            {
                receive_byte_array = listener.Receive(ref groupEP);
                received_data = Encoding.ASCII.GetString(receive_byte_array, 0, receive_byte_array.Length);
                string formattedString = String.Format("Server - Received a broadcast from {0} , Received data: {1}", groupEP.ToString(), received_data);
                System.Windows.Forms.MessageBox.Show(formattedString);

                if (pr == false)
                {
                    System.Windows.Forms.MessageBox.Show("Modalità pubblica.");
                    Socket sending_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    IPAddress send_to_address = groupEP.Address;
                    IPEndPoint sending_end_point = new IPEndPoint(send_to_address, 1501);
                    sending_socket.SendTo(Encoding.ASCII.GetBytes(/*"Mariano"*/nomeUtente), sending_end_point);
                    System.Windows.Forms.MessageBox.Show("Server - Sent!");
                    sending_socket.Close();
                }
                else
                    System.Windows.Forms.MessageBox.Show("Modalità privata.");
            }

            listener.Close(); //QUANDO DEVE CHIUDERSI IL SERVER?
        }

        public static void receiveNewFile(TcpClient client, string savePath, bool conferma)
        {
            /*********/
            NetworkStream netstream = null;
            string nomeFile = null;
            bool errore = false;
            FileStream Fs = null;
            /*if (conferma == true)
            {*/
            /*var confirmResult = System.Windows.Forms.MessageBox.Show("Are you sure to receive file ??",
                             "Confirm Receive!!",
                             MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes)
            {*/
            // If 'Yes', do something here.
            try
            {
                int BufferSize = 1024 * 1024;

                byte[] RecData = new byte[BufferSize];
                int RecBytes;
                //NetworkStream netstream = null;
                string Status = string.Empty;


                netstream = client.GetStream();//passare il client
                Status = "Connected to a client\n";
                //result = System.Windows.MessageBox.Show(message, caption, buttons);

                //file selection
                /*****/
                //ricevo nome file fino all'asterisco ma prima richiedo conferma di ricezione



                byte[] RecData_2 = new byte[1];


                while ((RecBytes = netstream.Read(RecData_2, 0, 1)) > 0)
                {
                    string tmp = System.Text.Encoding.UTF8.GetString(RecData_2);
                    if (tmp.CompareTo("*") == 0)
                    {
                        //nomeFile = nomeFile;
                        break;
                    }
                    else
                    {
                        nomeFile = nomeFile + tmp;
                    }
                }

                string[] fields = nomeFile.Split('\\'); //viene inviato il path assoluto del pc che sta inviando
                nomeFile = fields[fields.Length - 1];

                nomeFile = savePath + "\\" + nomeFile;
                //MessageBox.Show(nomeFile);

                /******/
                while (File.Exists(nomeFile))
                {
                    string s = null;
                    String[] words = nomeFile.Split('.');
                    s = words[0];
                    for (int i = 1; i < words.Length - 1; i++)
                    {

                        s = s + "." + words[i];
                    }
                    s += " - Copy." + words[words.Length - 1];
                    nomeFile = s;
                }
                /*******/
                //aggiungere "nuovaCartella\\" all'inizio del nome file per creare dentro cartella già esistente
                string SaveFileName = nomeFile;
                //MessageBox.Show(SaveFileName);
                if (SaveFileName != string.Empty)
                {
                    //DOPO AVER RICEVUTO IL NOMEFILE CHIEDO LA CONFERMA DI RICEZIONE
                    if (conferma == true)
                    {
                        var confirmResult = System.Windows.Forms.MessageBox.Show("Are you sure to receive " + nomeFile + " ?",
                                                            "Confirm Receive!!", MessageBoxButtons.YesNo);
                        if (confirmResult == DialogResult.Yes)
                        {
                            int totalrecbytes = 0;
                            Fs = new FileStream(SaveFileName, FileMode.OpenOrCreate, FileAccess.Write);

                            while ((RecBytes = netstream.Read(RecData, 0, RecData.Length)) > 0)
                            {
                                Fs.Write(RecData, 0, RecBytes);
                                totalrecbytes += RecBytes;
                                //string s = string.Format("Ricevuti {0} byte", totalrecbytes);
                                //MessageBox.Show(s);
                            }
                            Fs.Close();

                            System.Windows.MessageBox.Show(string.Format("Ricevuto '{0}'", nomeFile));
                        }
                        else
                        {
                            // If 'No', do something here.
                            //non ricevo il file
                            System.Windows.MessageBox.Show("Hai rifiutato la ricezione del file.");
                        }
                    }
                    else if (conferma == false)
                    {
                        int totalrecbytes = 0;
                        Fs = new FileStream(SaveFileName, FileMode.OpenOrCreate, FileAccess.Write);

                        while ((RecBytes = netstream.Read(RecData, 0, RecData.Length)) > 0)
                        {
                            Fs.Write(RecData, 0, RecBytes);
                            totalrecbytes += RecBytes;
                            //string s = string.Format("Ricevuti {0} byte", totalrecbytes);
                            //MessageBox.Show(s);
                        }
                        Fs.Close();

                        System.Windows.MessageBox.Show(string.Format("Ricevuto '{0}'", nomeFile));
                    }


                }//if su saveFilename
                else
                {
                    string s = string.Format("File non trovato.");
                    System.Windows.MessageBox.Show(s);
                }
            }//fine try
            catch (IOException ex)
            {
                System.Windows.MessageBox.Show("Trasferimento interrotto." + ex.ToString());
                errore = true;
            }

            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
            finally
            {
                try
                {
                    if (errore == true)
                    {
                        Fs.Close();
                        File.Delete(nomeFile);
                    }
                    netstream.Close();
                    client.Close();
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.ToString());
                }
            }

            /*}
            else
            {
                // If 'No', do something here.
                //non ricevo il file
                System.Windows.MessageBox.Show("Hai rifiutato la ricezione del file.");
            }*/
        }
        /*else if (conferma == false)//ricevo normalmente
        {   try
            {
                int BufferSize = 1024 * 1024;

                byte[] RecData = new byte[BufferSize];
                int RecBytes;
                //NetworkStream netstream = null;
                string Status = string.Empty;


                netstream = client.GetStream();//passare il client
                Status = "Connected to a client\n";
                //result = System.Windows.MessageBox.Show(message, caption, buttons);

                //file selection

                //ricevo nome file fino all'asterisco ma prima richiedo conferma di ricezione



                byte[] RecData_2 = new byte[1];


                while ((RecBytes = netstream.Read(RecData_2, 0, 1)) > 0)
                {
                    string tmp = System.Text.Encoding.UTF8.GetString(RecData_2);
                    if (tmp.CompareTo("*") == 0)
                    {
                        //nomeFile = nomeFile;
                        break;
                    }
                    else
                    {
                        nomeFile = nomeFile + tmp;
                    }
                }

                string[] fields = nomeFile.Split('\\'); //viene inviato il path assoluto del pc che sta inviando
                nomeFile = fields[fields.Length - 1];

                nomeFile = savePath + "\\" + nomeFile;
                //MessageBox.Show(nomeFile);

                while (File.Exists(nomeFile))
                {
                    string s = null;
                    String[] words = nomeFile.Split('.');
                    s = words[0];
                    for (int i = 1; i < words.Length - 1; i++)
                    {

                        s = s + "." + words[i];
                    }
                    s += " - Copy." + words[words.Length - 1];
                    nomeFile = s;
                }

                //aggiungere "nuovaCartella\\" all'inizio del nome file per creare dentro cartella già esistente
                string SaveFileName = nomeFile;
                //MessageBox.Show(SaveFileName);
                if (SaveFileName != string.Empty)
                {
                    int totalrecbytes = 0;
                    Fs = new FileStream(SaveFileName, FileMode.OpenOrCreate, FileAccess.Write);

                    while ((RecBytes = netstream.Read(RecData, 0, RecData.Length)) > 0)
                    {
                        Fs.Write(RecData, 0, RecBytes);
                        totalrecbytes += RecBytes;
                        //string s = string.Format("Ricevuti {0} byte", totalrecbytes);
                        //MessageBox.Show(s);
                    }
                    Fs.Close();

                    System.Windows.MessageBox.Show(string.Format("Ricevuto '{0}'", nomeFile));
                }
                else
                {
                    string s = string.Format("File non trovato.");
                    System.Windows.MessageBox.Show(s);
                }
            }
            catch (IOException ex)
            {
                System.Windows.MessageBox.Show("Trasferimento interrotto." + ex.ToString());
                errore = true;
            }

            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
            finally
            {
                try
                {
                    if (errore == true)
                    {
                        Fs.Close();
                        File.Delete(nomeFile);
                    }
                    netstream.Close();
                    client.Close();
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.ToString());
                }
            }
    }

    }*/
        /********************/
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
            System.Windows.MessageBox.Show(formattedString);

            Socket sending_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPAddress send_to_address = groupEP.Address;
            IPEndPoint sending_end_point = new IPEndPoint(send_to_address, 1501);
            sending_socket.SendTo(Encoding.ASCII.GetBytes("NINOzzp"), sending_end_point);
            System.Windows.MessageBox.Show("Server - Sent!");
            sending_socket.Close();
            listener.Close();
        }
    }
}


