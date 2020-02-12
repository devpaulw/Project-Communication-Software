using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Windows;

namespace PCS.WPFClientInterface
{
    static class PcsNotifier
    {
        public static void Notify(Window window, BroadcastMessage message)
        {
            WindowFlasher.Flash(window);
        }

        private static class WindowFlasher
        {
            [DllImport("user32")] public static extern int FlashWindow(IntPtr hwnd, bool bInvert);

            public static void Flash(Window window)
            {
                WindowInteropHelper wih = new WindowInteropHelper(window);
                FlashWindow(wih.Handle, true);
            }
        }
    }
}
