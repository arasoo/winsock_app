using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Common.Models;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Server
{
    class Program
    {
        
        static TcpListener serverSocket = new TcpListener(2912);
    

        static void Main(string[] args)
        {
            
            //TcpListener tcpListener = new TcpListener(10);
            serverSocket.Start();
            Console.WriteLine("************Server APP************");
            Console.WriteLine("Hoe many clients are going to connect to this server?: 10");
            //int numberOfClientsYouNeedToConnect = int.Parse(Console.ReadLine());
            int numberOfClientsYouNeedToConnect = 10;
            for (int i = 0; i < numberOfClientsYouNeedToConnect; i++)
            {
                Thread newThread = new Thread(new ThreadStart(Listeners));
                newThread.Start();
            }

           // TaskService.Instance.AddTask("Send Email", QuickTriggerType.Hourly, "notepad.exe", "-a arg");
        }

        static void Listeners()
        {
           int requestCount = 0;
            TcpClient clientSocket = serverSocket.AcceptTcpClient();

            if (clientSocket.Connected)
            {
                Console.WriteLine("Client:" + clientSocket.Client.RemoteEndPoint + " now connected to server.");
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

                while (true)
                {


                    if (serverStream.DataAvailable)
                    {
                        Sale s = (Sale)formatter.Deserialize(serverStream); // you have to cast the deserialized object 

                        Console.WriteLine("Hi, I'm " + s.branch + " " + s.dateTran + " and I'm " + s.total + " years old!");


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

                    }


                }

                serverStream.Close();
        

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
    }
}
