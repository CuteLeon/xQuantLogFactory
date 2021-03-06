﻿using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace BatchHost.Utils
{
    public static class UnityUtils
    {
        /// <summary>
        /// 日志分析工具文件路径
        /// </summary>
        public static string WorkPath = typeof(LogFactory.BIZ.Processer.LogProcesserBase).Assembly.Location;

        /// <summary>
        /// 日志分析工具文件名称
        /// </summary>
        public static string WorkName = Path.GetFileName(WorkPath);

        /// <summary>
        /// 日志分析工具目录
        /// </summary>
        public static string WorkDirectory = Path.GetDirectoryName(WorkPath);

        /// <summary>
        /// 生成目录
        /// </summary>
        public static string BuildDirectory = Path.Combine(WorkDirectory, "Batches");

        [DllImport("user32.dll")]
        public static extern bool RedrawWindow(IntPtr hWnd, IntPtr lprc, IntPtr hrgn, uint flags);
        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        public const int HT_CAPTION = 0x2;
        public const int WM_SIZE = 0x5;
        public const int WM_NCCALCSIZE = 0x83;
        public const int WM_PAINT = 0xF;
        public const int WM_NCPAINT = 0x85;
        public const int WM_NCACTIVATE = 0x86;
        public const int WM_NCMOUSEMOVE = 0xA0;
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int WM_NCLBUTTONUP = 0xA2;
        public const int WM_NCLBUTTONDBCLK = 0xA3;
        public const int WM_NCRBUTTONDOWN = 0xA4;
        public const int WM_NCRBUTTONUP = 0xA5;
        public const int WM_NCRBUTTONDBCLK = 0xA6;
        public const int WM_NCMOUSEHOVER = 0x2A0;
        public const int WM_NCMOUSELEAVE = 0x2A2;
        //鼠标拖动边框相关常量
        public const int WM_NCHITTEST = 0x0084;
        public const int HT_LEFT = 10;
        public const int HT_RIGHT = 11;
        public const int HT_TOP = 12;
        public const int HT_TOPLEFT = 13;
        public const int HT_TOPRIGHT = 14;
        public const int HT_BOTTOM = 15;
        public const int HT_BOTTOMLEFT = 16;
        public const int HT_BOTTOMRIGHT = 17;

        //REDRAW
        public const int SIZE_MAXIMIZED = 0x2;
        public const uint RDW_INVALIDATE = 0x1;
        public const uint RDW_IUPDATENOW = 0x100;
        public const uint RDW_FRAME = 0x400;

        /// <summary>
        /// 注册以帮助鼠标拖动无边框窗体
        /// </summary>
        static public void MoveFormViaMouse(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage((sender is Form ? (sender as Form).Handle : (sender as Control).FindForm().Handle), WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
    }
}
