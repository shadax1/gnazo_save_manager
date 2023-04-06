using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace gnazo_app
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Process[] pname = Process.GetProcessesByName("gnazo");
            if (pname.Length == 0)
            {
                try
                {
                    Process.Start("gnazo.exe");
                }
                catch (Exception)
                {
                    MessageBox.Show("gnazo.exe wasn't found, make sure the gnazo_app.exe AND the 'saves' folder are both in the same location as gnazo.exe.");
                    Environment.Exit(1);
                }
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new gnazo_app());
        }
    }
}
