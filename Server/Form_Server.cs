using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Threading;
using Common.Models;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Data.SQLite;


namespace Server
{
    public partial class Form_Server : Form
    {
        static TcpListener serverSocket = new TcpListener(2912);
        static SQLiteConnection sqliteCon;
        static SQLiteCommand sqliteCom;
        static SQLiteDataAdapter sqliteDa;
        static SQLiteTransaction sqliteTran;
        static DataTable dt;
        BackgroundWorker bw = new BackgroundWorker();
        string pubIp ="";

        public Form_Server()
        {
            InitializeComponent();
            bw.WorkerReportsProgress = true;
            bw.WorkerSupportsCancellation = true;
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (bw.IsBusy != true)
            {
                bw.RunWorkerAsync();
            }
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            pubIp = new System.Net.WebClient().DownloadString("https://api.ipify.org");
         
        }
        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            lblServer.Text = pubIp + ":2912";
        }


        private void Form_Server_Load(object sender, EventArgs e)
        {
   
            try
            {
                if (!File.Exists("AppDB.db"))
                {
                    sqliteCon = new SQLiteConnection("Data Source=AppDB.db;Version=3;New=True;Compress=True;");
                    sqliteCon.Open();
                    sqliteCom = sqliteCon.CreateCommand();
                    sqliteCom.CommandText = "CREATE TABLE sales (id integer primary key, branch  varchar(2),date  DateTime,org  varchar(3),amount Decimal);";
                    sqliteCom.ExecuteNonQuery();
                }
                else
                {
                    sqliteCon = new SQLiteConnection("DataSource=AppDB.db;Version=3;");
                    sqliteCon.Open();
                }

                sqliteCon.Close();



                //TcpListener tcpListener = new TcpListener(10);
                serverSocket.Start();
                Console.WriteLine("************Server Online************");
                //int numberOfClientsYouNeedToConnect = int.Parse(Console.ReadLine());
                int numberOfClientsYouNeedToConnect = 30;
                for (int i = 0; i < numberOfClientsYouNeedToConnect; i++)
                {
                    Thread newThread = new Thread(new ThreadStart(Listeners));
                    newThread.Start();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Server");
            }

        }

        private void Listeners()
        {
            int requestCount = 0;
            TcpClient clientSocket = serverSocket.AcceptTcpClient();

            if (clientSocket.Connected)
            {
                string c = "";
                c = "Client:" + clientSocket.Client.RemoteEndPoint + " now connected to server.";

                this.Invoke((MethodInvoker)(() => listClient.Items.Add(c)));
                requestCount = 0;

                //ClientInfo clientInfo = new ClientInfo()
                //{
                //    socket = clientSocket,
                //    ip = clientSocket.Client.RemoteEndPoint.ToString(),
                //};
                NetworkStream serverStream = clientSocket.GetStream();
                IFormatter formatter = new BinaryFormatter();



                //byte[] outStream = Encoding.ASCII.GetBytes("PCK|SCAN|5025066840471");
                //NetworkStream serverStream = new NetworkStream(clientSocket.Client);

                while (clientSocket.Connected)
                {

                    try
                    {
                        if (serverStream.DataAvailable)
                        {
                            dt = new DataTable();
                            Sale s = (Sale)formatter.Deserialize(serverStream); // you have to cast the deserialized object 

                            sqliteCon.Open();

                            sqliteTran = sqliteCon.BeginTransaction();


                            sqliteCom = new SQLiteCommand();

                            sqliteCom.Connection = sqliteCon;
                            sqliteCom.Transaction = sqliteTran;
                            sqliteCom.CommandText = "SELECT amount FROM sales " +
                                                    "WHERE branch='" + s.branch + "'" +
                                                    "AND date='" + string.Format("{0:yyyy-MM-dd}", s.dateTran) + "';";

                            sqliteDa = new SQLiteDataAdapter();
                            sqliteDa.SelectCommand = sqliteCom;
                            sqliteDa.Fill(dt);

                            sqliteCon.Close();

                            if (dt.Rows.Count == 0)
                            {
                                sqliteCon.Open();

                                sqliteCom = new SQLiteCommand();
                                sqliteCom.Connection = sqliteCon;
                                sqliteCom.Transaction = sqliteTran;
                                sqliteCom.CommandText = "INSERT INTO sales(branch,date,org,amount) " +
                                                        "VALUES ('" + s.branch + "'," +
                                                                "'" + string.Format("{0:yyyy-MM-dd}", s.dateTran) + "'," +
                                                                "'" + s.org + "'," +
                                                                "'" + s.total + "')";


                                sqliteCom.ExecuteNonQuery();

                                sqliteCon.Close();
                            }
                            else
                            {
                                sqliteCon.Open();
                                sqliteCom = new SQLiteCommand();
                                sqliteCom.Connection = sqliteCon;
                                sqliteCom.Transaction = sqliteTran;
                                sqliteCom.CommandText = "UPDATE sales SET amount=" + s.total +
                                                        " WHERE branch='" + s.branch + "'" +
                                                        " AND date='" + string.Format("{0:yyyy-MM-dd}", s.dateTran) + "';";

                                sqliteCom.ExecuteNonQuery();
                                sqliteCon.Close();
                            }

                            sqliteTran.Commit();

                            //        requestCount = requestCount + 1;
                            //        serverStream.Write(outStream, 0, outStream.Length);
                            //        byte[] inStream = new byte[4096];
                            //        int bytesRead = serverStream.Read(inStream, 0, inStream.Length);

                            //        string returndata = Encoding.ASCII.GetString(inStream, 0, bytesRead);
                            //        returndata = returndata.Substring(0, returndata.IndexOf("$"));

                            //        //Console.WriteLine(" >> Data from client - " + returndata);
                            //        //string serverResponse = "Last Message from client" + returndata;

                            //        Console.WriteLine("Message recieved by client:" + clientSocket.Client.RemoteEndPoint);
                            //        if (returndata == "exit") { break; }

                            //        Byte[] sendBytes = Encoding.ASCII.GetBytes(returndata);
                            //        serverStream.Write(sendBytes, 0, sendBytes.Length);
                            //        serverStream.Flush();
                            //        Console.WriteLine(" >> " + returndata);

                            serverStream.Close();

                        }
                    }
                    catch(Exception ex)
                    {
                        // Attempt to roll back the transaction. 
                        try
                        {
                            sqliteTran.Rollback();
                        }
                        catch (Exception ex2)
                        {
                            throw ex2;
                        
                        }
                    }
                    
                }

            }
        }

        internal static bool IsConnected(TcpClient client)
        {

            // This is how you can determine whether a socket is still connected.
            bool blockingState = client.Client.Blocking;
            try
            {
                byte[] tmp = new byte[1];

                client.Client.Blocking = false;
                client.Client.Send(tmp, 0, 0);
            }
            catch (SocketException e)
            {
                // 10035 == WSAEWOULDBLOCK
                if (e.NativeErrorCode.Equals(10035))
                    Console.WriteLine("Still Connected, but the Send would block");
                else
                {
                    Console.WriteLine("Disconnected: error code {0}!", e.NativeErrorCode);
                }
            }
            finally
            {
                //restore original blocking state
                client.Client.Blocking = blockingState;
            }

            return client.Connected;
        }

        public static string GetPublicIP()
        {
            string url = "http://checkip.dyndns.org";
            System.Net.WebRequest req = System.Net.WebRequest.Create(url);
            System.Net.WebResponse resp = req.GetResponse();
            System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
            string response = sr.ReadToEnd().Trim();
            string[] a = response.Split(':');
            string a2 = a[1].Substring(1);
            string[] a3 = a2.Split('<');
            string a4 = a3[0];
            return a4;
        }

    }
}
