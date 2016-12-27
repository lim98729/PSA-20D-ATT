using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;

namespace PSA_Application
{
    static class Program
    {
        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool result = false;

            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            System.Threading.Mutex mutex = new System.Threading.Mutex(true, @"PSA20D_Attach_Mutex", out result);
            if (!result)
            {
                MessageBox.Show("Attach control SW is already RUNNING!\nIf this problem is not solved, please check Task Manager.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormStart());

            mutex.Close();
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            string dirName = "C:\\PROTEC\\Log\\CriticalError\\";
            string fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
            if (!Directory.Exists(dirName)) Directory.CreateDirectory(dirName);

            Exception ex = (Exception)e.ExceptionObject;
            StreamWriter fs = new StreamWriter(dirName + fileName, false);

            fs.WriteLine(ex.ToString());
            fs.Close();

            MessageBox.Show("알 수 없는 상황으로 프로그램을 종료합니다.");
            Application.Exit();
        }
    }
}
