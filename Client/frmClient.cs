using System;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Common.Models;
using Common.Services;
using DevOne.Security.Cryptography.BCrypt;

namespace Client
{
    public partial class frmClient : Form
    {

        TcpClient clientSocket = new TcpClient();

        public frmClient()
        {
            InitializeComponent();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            Sale s = new Sale("01", DateTime.Now, "101", 5000000);
            IFormatter formatter = new BinaryFormatter(); 

            NetworkStream serverStream = clientSocket.GetStream();

            if (serverStream.CanWrite)
            {
                formatter.Serialize(serverStream, s);
            }
            

            //byte[] outStream = Encoding.ASCII.GetBytes(txtText.Text + "$");

            //serverStream.Write(outStream, 0, outStream.Length);

            //serverStream.Flush();

            //byte[] inStream = new byte[4096];
            //int bytesRead = serverStream.Read(inStream, 0, inStream.Length);
            //string returndata = Encoding.ASCII.GetString(inStream, 0, bytesRead);

            //msg(returndata);
            txtText.Text = "";
            txtText.Focus();
      
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {

            if (btnConnect.Text=="Connect")
            {
                IPHostEntry ipHost = Dns.GetHostEntry(txtHost.Text.Trim());
                IPAddress ipAddr = ipHost.AddressList[0];
                IPEndPoint ipEndPoint = new IPEndPoint(ipAddr,Convert.ToInt32(txtPort.Text.Trim()));

                try
                {
                    if (clientSocket.Connected == false)
                    {
                        clientSocket.Connect(ipEndPoint);
                        label1.Text = "Client Socket - Server Connected ...";
                        btnSend.Enabled = true;
                        btnConnect.Text = "Disconnect";
                    }
                    else
                    {
                        MessageBox.Show("Client Connected");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to connect to server at " + ipAddr + ":2912", ipHost.HostName);
                }
     
            }
            else
            {
                NetworkStream serverStream = clientSocket.GetStream();
                clientSocket.Close();
                serverStream.Close();
                serverStream.Dispose();

                label1.Text = "Client Socket - Server disconnected ...";
                clientSocket = new TcpClient();

                btnSend.Enabled = false;
                btnConnect.Text = "Connect";
            }
           
        
        
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            clientSocket.Close();
            Close();
        }

        private void txtPort_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
                (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        private void frmClient_Load(object sender, EventArgs e)
        {
            if(!File.Exists(Application.StartupPath + "\\app.ini"))
            {
                File.Create(Application.StartupPath + "\\app.ini");

            }

            var appIni = new IniFile(Application.StartupPath + "\\app.ini");

            if (!appIni.KeyExists("HOST", "SQL"))
            {
                appIni.Write("HOST", "192.168.10.44", "SQL");
                appIni.Write("DATABASE", "TM_HO", "SQL");
                appIni.Write("USER", "bsp", "SQL");
                appIni.Write("PASSWORD", "1992045", "SQL");
            }

            if (!appIni.KeyExists("SERVER", "WINSOCK"))
            {
                appIni.Write("SERVER", "127.0.0.1", "WINSOCK");
                appIni.Write("PORT", "2912", "WINSOCK");
 
            }

            string salt = BCryptHelper.GenerateSalt(6);

            var hashedPassword = BCryptHelper.HashPassword("1992045", salt);

            MessageBox.Show(hashedPassword);

        }
    }
}

