using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Gomoku1._0_Alpha
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());
            //Application.Run(new testForm());
            Application.Run(new GomokuForm());
        }
    }
}