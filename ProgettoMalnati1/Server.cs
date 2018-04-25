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
using System.IO.Compression; 
using System.Drawing;             


namespace ProgettoMalnati1
{
    public class Server
    {
        private volatile bool privato;
        public bool conferma;//usata nella funzione statica del primo thread
        public string savePath, nomeUtente;
        public Uri image;
        //public ManualResetEvent oSignalEvent;
        NotifyIcon myIcon;
        System.Windows.Forms.ContextMenu myMenu;
        System.Windows.Forms.MenuItem myMenuItem;
        System.Windows.Forms.MenuItem myMenuItem2;
        public static string choosenPath = null;
        public static bool confRic = false; public static bool defaultPath;
        Thread notifyThread;
        //a seguire ttto ciò che e' usato per la seconda icona nella task bar
        Thread notifyThread_;
        NotifyIcon myIcon_;
        System.Windows.Forms.ContextMenu myMenu_;
        System.Windows.Forms.MenuItem myMenuItem_;
        System.Windows.Forms.MenuItem myMenuItem2_;



        public Server(bool privato, string savePath, string nomeUtente, bool conferma, Uri image)
        {
            this.privato = privato;//posso annunciarmi sulla rete come server
            this.savePath = savePath;
            this.nomeUtente = nomeUtente;
            this.conferma = conferma;
            this.image = image;

            bool defaultPath = false;

            /*******CREAZIONE ICONA NELLA TASKBAR******/
            //nel server per gestire la variabile bool "privato"
            notifyThread = new Thread(
               delegate ()
               {

                   myMenu = new System.Windows.Forms.ContextMenu();
                   myMenuItem = new System.Windows.Forms.MenuItem("Impostazioni di condivisione");
                   myMenu.MenuItems.Add(0, myMenuItem);

                   if (privato == false)
                       myMenuItem2 = new System.Windows.Forms.MenuItem("Stato: online");
                   else
                       myMenuItem2 = new System.Windows.Forms.MenuItem("Stato: offline");
                   myMenu.MenuItems.Add(1, myMenuItem2);

                   myIcon = new NotifyIcon()
                   {
                       Icon = new Icon(AppDomain.CurrentDomain.BaseDirectory + "\\immagineIcona.ico"),
                       ContextMenu = myMenu,
                       Text = "AppMalnati"
                   };
                   if (this.savePath.Equals(AppDomain.CurrentDomain.BaseDirectory))
                       defaultPath = true;
                   myMenuItem.Click += new EventHandler((s2, e2) => ChangeProperties(s2, e2, this.conferma, defaultPath, this.nomeUtente, this.savePath));


                   myIcon.Visible = true;
                   System.Windows.Forms.Application.Run();
               }
            );


            notifyThread.Start();

            // CREARE UN NUOVO THREAD ASSOCIATO ALLA SECONDA ICONA MA FARLA PARTIRE CON VISIBLE=FALSE
            notifyThread_ = new Thread(
               delegate ()
               {
                   myMenu_ = new System.Windows.Forms.ContextMenu();
                   myMenuItem_ = new System.Windows.Forms.MenuItem("Impostazioni di condivisione");
                   myMenu_.MenuItems.Add(0, myMenuItem_);

                   if (privato == false)
                       myMenuItem2_ = new System.Windows.Forms.MenuItem("Stato: offline");
                   else
                       myMenuItem2_ = new System.Windows.Forms.MenuItem("Stato: online");
                   myMenu_.MenuItems.Add(1, myMenuItem2_);

                   myIcon_ = new NotifyIcon()
                   {
                       Icon = new Icon(AppDomain.CurrentDomain.BaseDirectory + "\\immagineIcona.ico"),
                       ContextMenu = myMenu_,
                       Text = "AppMalnati"
                   };

                   if (this.savePath.Equals(AppDomain.CurrentDomain.BaseDirectory))
                       defaultPath = true;
                   myMenuItem_.Click += new EventHandler((s2, e2) => ChangeProperties(s2, e2, this.conferma, defaultPath, this.nomeUtente, this.savePath));

                   myIcon_.Visible = false;
                   System.Windows.Forms.Application.Run();
               }
            );

            notifyThread_.Start();

            /********************************************/


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

        //funziona associata al click della prima voce del menu dell'icona nella task bar
        private void ChangeProperties(object s, EventArgs e, bool confRic2, bool defaultPath2, string nomeU, string choosenPath2)
        {
            //metodo associato al click della voce "Impostazioni di condivisione"
            //finestra con le impostazioni attuali e possibilità di cambiare lo stato
            try
            {
                //nome utente, conferma di ricezione, default path
                string confRicezione = null;
                string defPath = null;

                if (confRic2 == true)
                    confRicezione = "conferma di ricezione impostata";
                else
                    confRicezione = "conferma di ricezione NON impostata";

                if (defaultPath2 == false)
                    defPath = "percorso di salvataggio SCELTO dall'utente -> " + choosenPath2;
                else
                    defPath = "percorso di salvataggio di default -> " + choosenPath2;
                var confirmResult = System.Windows.Forms.MessageBox.Show("Nome: " + nomeU + "\n\n" + "Conferma di ricezione: " + confRicezione +
                    "\n\n" + "Default path: " + defPath + "\n\n" + "Vuoi cambiare il tuo stato ?", "Impostazioni di condivisione", MessageBoxButtons.YesNo);

               
                //se cambio stato allora modifico il valore di this.privato
                if (confirmResult == DialogResult.Yes)
                {
                    //SE VISIBLE DELLA PRIMA ICONA = FALSE ALLORA LO METTO A TRUE E LA SECONDA ICONA A FALSE
                    //ALTRIMENTI SE VISIBLE DELLA PRIMA ICONA = TRUE ALLORA LA METTO A FALSE E LA SECONDA LA METTO A TRUE 

                    //cambio della variabile "privato"
                    if (privato == false)
                    {
                        this.privato = true;
                        if (myIcon.Visible == true)
                        {
                            myIcon.Visible = false;
                            myIcon_.Visible = true;
                        }
                        else if (myIcon_.Visible == true)
                        {
                            myIcon_.Visible = false;
                            myIcon.Visible = true;
                        }

                    }
                    else
                    {
                        this.privato = false;
                        if (myIcon.Visible == true)
                        {
                            myIcon.Visible = false;
                            myIcon_.Visible = true;
                        }
                        else if (myIcon_.Visible == true)
                        {
                            myIcon_.Visible = false;
                            myIcon.Visible = true;
                        }
                    }
                }

               
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message + "\n problema thread");
            }

        }


        //prima funzione del thread
        public void startBroadcastSocketStatic(bool pr, string nomeUtente)
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

                if (this.privato == false)
                {
                    System.Windows.Forms.MessageBox.Show("Modalità pubblica.");
                    Socket sending_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    IPAddress send_to_address = groupEP.Address;
                    IPEndPoint sending_end_point = new IPEndPoint(send_to_address, 1501);
                    sending_socket.SendTo(Encoding.ASCII.GetBytes(nomeUtente+"*"), sending_end_point);

                    var imageToSend = Image.FromFile(image.AbsolutePath);
                    MemoryStream ms = new MemoryStream();
                    imageToSend.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    var msA = ms.ToArray();
                    sending_socket.SendTo(msA, sending_end_point);

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

            string f_d = null; //riceverà "zero" o "uno"
            string st = null;



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
                //ricevo flagDirectory e nome file fino all'asterisco ma prima richiedo conferma di ricezione



                byte[] RecData_2 = new byte[1];


                byte[] RecData_3 = new byte[1];

                //string f_d = null; //riceverà "zero" o "uno"
                while ((RecBytes = netstream.Read(RecData_3, 0, 1)) > 0)
                {
                    string tmp = System.Text.Encoding.UTF8.GetString(RecData_3);
                    if (tmp.CompareTo("*") == 0)
                    {
                        break;
                    }
                    else
                    {
                        f_d = f_d + tmp;
                    }

                }
                System.Windows.MessageBox.Show(f_d);
                if (f_d.Equals("uno"))
                    System.Windows.MessageBox.Show("Ho ricevuto una cartella.");
                else
                    System.Windows.MessageBox.Show("Ho ricevuto un file.");



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

                            //effettuo l'eventuale decompressione
                            if (f_d.Equals("uno"))
                            {

                                string[] w = SaveFileName.Split('.');
                                st = w[0];//nome del file escluso .zip
                                while (Directory.Exists(st))//cerco una cartella con stesso nome e non il file
                                {
                                    st = st + "-Copy";
                                }
                                ZipFile.ExtractToDirectory(nomeFile, st);
                                //elimino il .zip
                                File.Delete(SaveFileName);
                            }


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

                        //effettuo l'eventuale decompressione
                        if (f_d.Equals("uno"))
                        {
                            string[] w = SaveFileName.Split('.');
                            st = w[0];//nome del file escluso .zip
                            while (Directory.Exists(st))//cerco una cartella con stesso nome e non il file
                            {
                                st = st + "-Copy";
                            }
                            ZipFile.ExtractToDirectory(nomeFile, st);
                            //elimino il .zip
                            File.Delete(SaveFileName);
                        }

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
                        if (File.Exists(nomeFile))
                            File.Delete(nomeFile);//se il nomeFile esiste (un file qualsiasi, può essere anche lo zip)
                        //se ho ricevuto una cartella f_d == 1 elimino anche la cartella creata ---> Directory.Delete(nome della cartella creata in st) se esiste
                        if (f_d.Equals("uno") && Directory.Exists(st))
                            Directory.Delete(st);

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
        
        /********************/
        
    }
}


