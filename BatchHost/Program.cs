using System;
using System.IO;
using System.Windows.Forms;
using BatchHost.Utils;

namespace BatchHost
{
    internal static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        private static void Main()
        {
            if (!Directory.Exists(UnityUtils.BuildDirectory))
            {
                try
                {
                    Directory.CreateDirectory(UnityUtils.BuildDirectory);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "创建生成目录失败！");
                }
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new BatchHostForm());
        }
    }
}
