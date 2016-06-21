using System;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Drawing;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Common.Models;
using Common.Services;
using DevOne.Security.Cryptography.BCrypt;
using System.Data.SqlClient;
using System.Data;
using System.Threading;

namespace Client
{
    public partial class frmClient : Form
    {

        TcpClient clientSocket = new TcpClient();
        string pwdhash = "";
        string salt = BCryptHelper.GenerateSalt(6);
        SqlConnection cn;
        SqlCommand cm;
        SqlDataAdapter da;
        string query = "";
        DataTable dt;

        public frmClient()
        {
            InitializeComponent();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {

            try
            {
                var appIni = new IniFile(Application.StartupPath + "\\app.ini");
                decimal amount = 0;

                //string value = "Document 1";
                //if (InputBox("New document", "New document name:", ref value) == DialogResult.OK)
                //{
                //    MessageBox.Show(value);
                //}

                //kroscek password sql di app.ini
                string sqlconstr = "Data Source=" + appIni.Read("HOST", "SQL") +
                                    ";Initial Catalog=" + appIni.Read("DATABASE", "SQL") +
                                    ";User Id=" + appIni.Read("USER", "SQL") +
                                    ";Password=1992045" +
                                    ";Persist Security Info=true";

                cn = new SqlConnection(sqlconstr);
                cn.Open();

                query = "SELECT SUM(salesamount)amt FROM " +
                                 appIni.Read("DATABASE", "SQL") + ".dbo.tpayrech " +
                                 "WHERE documentdate = '" + string.Format("{0:yyyy-MM-dd}", DateTime.Now) + "'";
                cm = new SqlCommand();
                cm.Connection = cn;
                cm.CommandType = CommandType.Text;
                cm.CommandText = query;

                dt = new DataTable();

                da = new SqlDataAdapter();
                da.SelectCommand = cm;
                da.Fill(dt);

                if (dt.Rows.Count == 0)
                {
                    amount = 0;
                }
                else
                {
                    amount = dt.Rows[0].Field<Decimal>(0);
                }



                Sale s = new Sale(appIni.Read("BRANCH", "PROFILE"), DateTime.Now, appIni.Read("SALES ORG", "PROFILE"), amount);
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Window");
            }
            finally
            {
                cn.Close();
            }


        }

        private void btnConnect_Click(object sender, EventArgs e)
        {

            if (btnConnect.Text == "Connect")
            {
                IPHostEntry ipHost = Dns.GetHostEntry(txtHost.Text.Trim());
                IPAddress ipAddr = ipHost.AddressList[0];
                IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, Convert.ToInt32(txtPort.Text.Trim()));

                try
                {
                    if (clientSocket.Connected == false)
                    {
                        clientSocket.Connect(ipEndPoint);
                        label1.Text = "Client Socket - Server Connected ...";
                        btnSend.Enabled = true;
                        btnConnect.Text = "Disconnect";

                        // set last setting winsock if connected
                        var appIni = new IniFile(Application.StartupPath + "\\app.ini");

                        appIni.Write("SERVER", txtHost.Text.Trim(), "WINSOCK");
                        appIni.Write("PORT", txtPort.Text.Trim(), "WINSOCK");

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

            if (!File.Exists(Application.StartupPath + "\\app.ini"))
            {
                File.Create(Application.StartupPath + "\\app.ini");

            }

            var appIni = new IniFile(Application.StartupPath + "\\app.ini");
            if (!appIni.KeyExists("BRANCH", "PROFILE"))
            {
                appIni.Write("BRANCH", "01", "PROFILE");
                appIni.Write("SALES ORG", "101", "PROFILE");


            }

            if (!appIni.KeyExists("HOST", "SQL"))
            {
                appIni.Write("HOST", "192.168.10.44", "SQL");
                appIni.Write("DATABASE", "TM_HO", "SQL");
                appIni.Write("USER", "bsp", "SQL");

                pwdhash = "1992045" + "A-r4450!";
                var hashedPassword = BCryptHelper.HashPassword(pwdhash, salt);
                appIni.Write("PASSWORD", hashedPassword, "SQL");
            }

            if (!appIni.KeyExists("SERVER", "WINSOCK"))
            {
                appIni.Write("SERVER", "127.0.0.1", "WINSOCK");
                appIni.Write("PORT", "2912", "WINSOCK");

            }


            //if(!DoesPasswordMatch(appIni.Read("PASSWORD", "SQL"), "1992045") == true)
            // {
            //     MessageBox.Show("Salah");
            // }
            // else
            // {
            //     MessageBox.Show("Betul");
            // }

            txtHost.Text = appIni.Read("SERVER", "WINSOCK");
            txtPort.Text = appIni.Read("PORT", "WINSOCK");


        }

        private void frmClient_Activated(object sender, EventArgs e)
        {
            btnConnect.PerformClick();

            if (clientSocket.Connected)
            {
                btnSend.PerformClick();
            }

            Thread.Sleep(5000);

            btnExit.PerformClick();
        }

        private bool DoesPasswordMatch(string hashedPwdFromIni, string userEnteredPassword)
        {
            return BCryptHelper.CheckPassword(userEnteredPassword + "A-r4450!", hashedPwdFromIni);
        }
        public static DialogResult InputBox(string title, string promptText, ref string value)
        {
            Form form = new Form();
            Label label = new Label();
            TextBox textBox = new TextBox();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();

            form.Text = title;
            label.Text = promptText;
            textBox.Text = value;

            buttonOk.Text = "OK";
            buttonCancel.Text = "Cancel";
            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;

            label.SetBounds(9, 20, 372, 13);
            textBox.SetBounds(12, 36, 372, 20);
            buttonOk.SetBounds(228, 72, 75, 23);
            buttonCancel.SetBounds(309, 72, 75, 23);

            label.AutoSize = true;
            textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            form.ClientSize = new Size(396, 107);
            form.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
            form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;

            DialogResult dialogResult = form.ShowDialog();
            value = textBox.Text;
            return dialogResult;
        }

    }
}

