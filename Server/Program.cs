﻿using System;
using System.Text;
using System.Net;
using System.Windows.Forms;

namespace Server
{
    static class Program
    {


        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form_Server());
        }

    }
}
