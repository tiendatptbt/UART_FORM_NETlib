using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UART_FORM_NETlib
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
            Application.Run(new Form1());
        }
    }
    static class Data
    {
        public static double[] CirArrayX = new double[100];
        public static double[] CirArrayY = new double[100];
        public static double[] CirArrayZ = new double[100];
        public static double serialRAW ;
        public static Int16[] data = new Int16[3];
        public static Int16[] datatest = new Int16[3];
        public static void method()
        {

        }
    }
}
