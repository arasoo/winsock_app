using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Windows.Forms;

namespace send_email
{
    public partial class Form1 : Form
    {
        static SQLiteConnection sqliteCon;
        static SQLiteCommand sqliteCom;
        static SQLiteDataAdapter sqliteDa;
        static DataTable dt;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

           

        }

        private void Form1_Activated(object sender, EventArgs e)
        {
            try
            {
                string query = "";
                sqliteCon = new SQLiteConnection("DataSource=AppDB.db;Version=3;");
                sqliteCon.Open();
                query = "SELECT branch,org,amount FROM sales " +
                        "WHERE date = '" + string.Format("{0:yyyy-MM-dd}", DateTime.Now) + "' " +
                        "ORDER BY branch ASC";

                sqliteCom = new SQLiteCommand();
                sqliteCom.Connection = sqliteCon;
                sqliteCom.CommandType = CommandType.Text;
                sqliteCom.CommandText = query;

                dt = new DataTable();

                sqliteDa = new SQLiteDataAdapter();
                sqliteDa.SelectCommand = sqliteCom;
                sqliteDa.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    var fromAddress = new MailAddress("s.setia@bsp-groups.com", "SYSTEM OMSET");
                    var toAddress = new MailAddress("f.pranoto@bsp-groups.com");
                    var copyAddress1 = new MailAddress("b.pranoto@bsp-groups.com");
                    var copyAddress2 = new MailAddress("s.setia@bsp-groups.com");

                    const string fromPassword = "mumuweka1229";
                    const string subject = "OMSET TMBOOKSTORE";
                    string code, name = "";

                    string body = "";

                    body += "OMSET TMBOOKSTORE" + "\r\n";
                    body += string.Format("{0:dd MMM yyyy HH:mm:ss tt}", DateTime.Now) + "\r\n";
                    body += "============================" + "\r\n" + "\r\n";

                    for (int i = 0; i <= dt.Rows.Count - 1; i++)
                    {
                        code = dt.Rows[i].Field<String>(0);
                        if (code == "01")
                        {
                            name = "1. TM DEPOK TOWN SQUARE";
                        }
                        else if (code == "02")
                        {
                            name = "2. TM POINS SQUARE";
                        }
                        else if (code == "03")
                        {
                            name = "3. TM SUPERINDO KEDOYA";
                        }
                        else if (code == "05")
                        {
                            name = "4. TM DMALL";
                        }
                        else if (code == "06")
                        {
                            name = "5. TM PLAZA CIBUBUR";
                        }
                        else if (code == "07")
                        {
                            name = "6. PAMERAN POINS LT. UG";
                        }
                        else
                        {
                            name = "7. PAMERAN DMALL LT. UG";
                        }


                        body += name + "\r\n";
                        body += "-----------------------" + "\r\n";
                        body += "TOKO : " + string.Format("{0:N0}", Convert.ToDouble(dt.Rows[i].Field<decimal>(2)));
                        body += "\r\n" + "\r\n" + "\r\n";
                    }

                    var smtp = new SmtpClient
                    {
                        Host = "smtp.gmail.com",
                        Port = 587,
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                    };
                    using (var message = new MailMessage(fromAddress, toAddress)
                    {
                        Subject = subject,
                        Body = body
                    })
                    {
                        message.CC.Add(copyAddress1);
                        message.CC.Add(copyAddress2);
                        smtp.Send(message);
                    }


                }

                Thread.Sleep(5000);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Window");
            }

        }
    }
}
