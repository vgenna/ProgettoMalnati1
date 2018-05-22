using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.IO.Compression;          

namespace ProgettoMalnati1
{
    /// <summary>
    /// Interaction logic for WinProgBar.xaml
    /// </summary>
    public partial class WinProgBar : Window
    {
        private Client c;
        private bool error;

        public WinProgBar()
        {
            InitializeComponent();
            this.Show();
        }

        public WinProgBar(Client c)
        {
            try
            {
                InitializeComponent();

                this.c = c;
                error = false;

                string nomefile = c.filename;

                int flagDirectory = 0;//indica se una nuova directory.zip e' stata creata, in questo caso vale 1
                if (Directory.Exists(c.filename))
                {

                    //System.Windows.MessageBox.Show("Invio di una cartella");
                    nomefile = c.filename + ".zip";//nomefile sara' la stringa ottenuta dallo zip
                    /*
                     *compressione della cartella con creazione della directory e poi successiva eliminazione altrimenti non puo' essere reinviata
                     */

                    /*elimino un eventuale file .zip avente lo stesso nome della cartella che devo comprimere e inviare altrimenti 
                      ci sarà una eccezione; questi file possono essere presenti dopo aver fatto abort */
                    /* si può mettere la delete anche nel IF error == true della sendFileTCP*/
                    if (File.Exists(nomefile))
                        File.Delete(nomefile);

                    ZipFile.CreateFromDirectory(c.filename, nomefile);
                    flagDirectory = 1;
                }



                //QUESTO DOVRA' ESSERE FATTO NEL COSTRUTTORE DEL CLIENT, PERCHE' CI SARA' GIA' LI' IL PATH
                long length = new FileInfo(nomefile).Length;
                c.nBytesTot = length * c.usersToShare.Count();

                this.Show();

                foreach (OtherUser ou in c.usersToShare)
                {
                    
                    //Action workAction = delegate
                    //{
                    BackgroundWorker worker = new BackgroundWorker();
                    worker.WorkerReportsProgress = true;
                    worker.DoWork += delegate
                    {
                        sendFileTCP(ou.Address.ToString(), 1500, nomefile, worker, flagDirectory);
                    };

                    worker.ProgressChanged += worker_ProgressChanged;
                    worker.ReportProgress(-1, c.nBytesTot); //setto il valore che corrisponde al 100%

                   worker.RunWorkerCompleted += delegate //potrei levarlo e fare stampare "file Condiviso" solo appena esce dal for
                    {
                        this.Close();
                        if (error == false)
                            System.Windows.MessageBox.Show(string.Format("File {0} condiviso!", nomefile));
                        else
                            System.Windows.MessageBox.Show(string.Format("File {0} NON condiviso!", nomefile));
                    };
                    worker.RunWorkerAsync(); 
                                             //}

                    //Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, workAction);
                }
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.ToString());
            }
        }


        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == -1)
                pBar.Maximum = Convert.ToInt64(e.UserState);
            else
            {
                pBar.Value += e.ProgressPercentage;
                textBox.Text = "Time Left: " + e.UserState + "ms";
            }
                

        }

        public void sendFileTCP(string IP_addr, Int32 port_number, string nome_file, BackgroundWorker worker, int flagDirectory)
        {
            TcpClient client = null;
            NetworkStream netstream = null;
            FileStream Fs = null;
            
            try
            {
                //System.Windows.MessageBox.Show("Invio di una FILE");
                double timeLeft, lastSpeed, averageSpeed=0;
                int amountLeft, amountProcessed = 0, timeTaken = 0, preTransferTime, postTransferTime;
                client = new TcpClient(IP_addr, port_number);
                netstream = client.GetStream();
                //int BufferSize = 1024*1024;/**dimensione del pacchetto inviato**/
                int BufferSize = (int) new FileInfo(nome_file).Length/50; //problema per invii >400GB(int non contiene un numero >4miliardi)
                if (BufferSize < 100)
                    BufferSize = 100;

                int sent = 0;

                Fs = new FileStream(nome_file, FileMode.Open, FileAccess.Read);/**apro il file in lettura**/
                int packets_number = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(Fs.Length) / Convert.ToDouble(BufferSize)));
                /**il numero di pacchetti da inviare è pari alla grandezza totale del file diviso la 
            grandezza del buffer**/

                int lunghezza_file = (int)Fs.Length, dim_pacchetto_corrente;

                //invio il valore del flagDirectory per far capire al server se deve decomprimere lo zip
                string f_d = "";
                if (flagDirectory == 0)
                    f_d = "zero*";
                else
                    f_d = "uno*";
                byte[] stringFlagDirectory = Encoding.ASCII.GetBytes(f_d);
                netstream.Write(stringFlagDirectory, 0, (int)f_d.Length);

                //invia nome del file
                string stringInvio = nome_file + "*";
                byte[] stringBytes = Encoding.ASCII.GetBytes(stringInvio);
                netstream.Write(stringBytes, 0, nome_file.Length + 1);

                for (int i = 0; i < packets_number && !error; i++)
                {
                    preTransferTime = System.Environment.TickCount;
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
                    postTransferTime = System.Environment.TickCount;

                    sent = dim_pacchetto_corrente;
                    amountLeft = lunghezza_file;
                    amountProcessed += dim_pacchetto_corrente;                  
                    timeTaken = postTransferTime - preTransferTime;

                    if (timeTaken == 0) timeTaken = 16; 
                    lastSpeed = dim_pacchetto_corrente / timeTaken;
                    if (averageSpeed == 0)
                        averageSpeed = lastSpeed;

                    averageSpeed = (float)(0.005 * lastSpeed + (1 - 0.005) * averageSpeed);
                    timeLeft = Math.Ceiling(amountLeft / averageSpeed);

                    worker.ReportProgress(sent, timeLeft);
                }
                if(error)
                    throw new Exception("Trasferimento interrotto!");
                else
                {
                    client.Close();
                    Fs.Close();
                    netstream.Close();
                }

            }
            catch (Exception e)

            {
                System.Windows.MessageBox.Show(e.Message);
            }
            finally
            {
                //cancello il file compresso ottenuto dalla cartella da inviare a prescindere dal fatto che abbia fatto abort
                if (flagDirectory == 1 && File.Exists(nome_file))
                    File.Delete(nome_file);

                this.Close(); //non esegue il codice dopo-->potremmo inviare la dimensione del file e fare il check dell'errore solo sul server
                System.Windows.MessageBox.Show("Chiudendo coseeeee");
                client.Close();
                Fs.Close();       
                netstream.Close();
            }
        }

        private void AbortButton_Click(object sender, RoutedEventArgs e)
        {
            //netstream.Close();
            //client.Close();
            //throw new Exception("Trasferimento interrotto!");
            error = true;
            //this.Close();
        }
    }
}
