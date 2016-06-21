namespace Server
{
    partial class Form_Server
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.listClient = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblServer = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // listClient
            // 
            this.listClient.FormattingEnabled = true;
            this.listClient.Location = new System.Drawing.Point(2, 61);
            this.listClient.Name = "listClient";
            this.listClient.Size = new System.Drawing.Size(343, 342);
            this.listClient.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 14);
            this.label1.TabIndex = 1;
            this.label1.Text = "LISTEN :";
            // 
            // lblServer
            // 
            this.lblServer.AutoSize = true;
            this.lblServer.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblServer.ForeColor = System.Drawing.Color.White;
            this.lblServer.Location = new System.Drawing.Point(69, 14);
            this.lblServer.Name = "lblServer";
            this.lblServer.Size = new System.Drawing.Size(171, 25);
            this.lblServer.TabIndex = 2;
            this.lblServer.Text = "127.0.0.1:2912";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // Form_Server
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.SteelBlue;
            this.ClientSize = new System.Drawing.Size(345, 405);
            this.ControlBox = false;
            this.Controls.Add(this.lblServer);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listClient);
            this.Name = "Form_Server";
            this.Text = "Form_Server";
            this.Load += new System.EventHandler(this.Form_Server_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listClient;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblServer;
        private System.Windows.Forms.Timer timer1;
    }
}