﻿using System;
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
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ProgettoMalnati1
{
    /// <summary>
    /// Interaction logic for WinProgBar.xaml
    /// </summary>
    public partial class WinProgBar : Window
    {
        private Client c;

        public WinProgBar()
        {
            InitializeComponent();
            this.Show();
        }

        public WinProgBar(Client c)
        {
            InitializeComponent();

            this.c = c;

            string nomefile = "prova.txt";

            //QUESTO DOVRA' ESSERE FATTO NEL COSTRUTTORE DEL CLIENT, PERCHE' CI SARA' GIA' LI' IL PATH
            long length = new System.IO.FileInfo(nomefile).Length;
            c.nBytesTot = length * c.usersToShare.Count();

            this.Show();

            foreach (OtherUser ou in c.usersToShare)
            {
                //VEDERE COME SI FANNO I THREAD NEL WINOTHERUSERS NON MODIFICATO SU GITHUB E PROVARE A FARE 3-4 
                //THREAD CHE AGGIORNANO LA STESSA PROGRESS BAR (CON JOIN ALLA FINE)
                //this.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate ()
                //{
                //    this.progressBar.Value = 20; // Do all the ui thread updates here
                //}));


                //Action workAction = delegate
                //{
                BackgroundWorker worker = new BackgroundWorker();
                worker.WorkerReportsProgress = true;
                worker.DoWork += delegate
                {
                    sendFileTCP(ou.Address.ToString(), 1500, nomefile, worker);
                };

                worker.ProgressChanged += worker_ProgressChanged;
                worker.ReportProgress(-1, c.nBytesTot); //setto il valore che corrisponde al 100%

                worker.RunWorkerCompleted += delegate
                {
                    MessageBox.Show(string.Format("File {0} condiviso!", nomefile));
                    //this.Close();
                };
                worker.RunWorkerAsync(); //PARTE MA NON RITORNA PIù BOHHHH
                //}

                //Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, workAction);
            }           
        }

        /* private void Window_ContentRendered(object sender, EventArgs e)
         {
             BackgroundWorker worker = new BackgroundWorker();
             worker.WorkerReportsProgress = true;
             worker.DoWork += worker_DoWork;
             worker.ProgressChanged += worker_ProgressChanged;
             MessageBox.Show("?");
             worker.RunWorkerAsync();
         }

         void worker_DoWork(object sender, DoWorkEventArgs e)
         {
             for (int i = 0; i < 100; i++)
             {
                 (sender as BackgroundWorker).ReportProgress(i);
                 Thread.Sleep(100);
             }
         }

         void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
         {
             pBar.Value = e.ProgressPercentage;
         }*/

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == -1)
                pBar.Maximum = Convert.ToInt32(e.UserState);
            else
                pBar.Value = e.ProgressPercentage;
        }

        public void sendFileTCP(string IP_addr, Int32 port_number, string nome_file, BackgroundWorker worker)
        {
            TcpClient client = null;
            NetworkStream netstream = null;
            try
            {
                client = new TcpClient(IP_addr, port_number);
                netstream = client.GetStream();
                int BufferSize = 1024*1024;/**dimensione del pacchetto inviato**/
                int sent = 0;

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
                    //pBar.Value += (dim_pacchetto_corrente / nBytesTot) * 100;
                    sent += dim_pacchetto_corrente;
                    worker.ReportProgress(sent);

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
    }
}
