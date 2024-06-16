﻿using System.Runtime.InteropServices;

namespace nardnob.InputTracker.WindowGrabber
{
    public static class Grabber
    {
        [DllImport("User32.dll")]
        private static extern bool ReleaseCapture();

        [DllImport("User32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        /// <summary>
        /// Calling this method "grips" the window while the mouse button is held.
        /// </summary>
        public static void Grab(IntPtr windowHandle)
        {
            //WM_NCLBUTTONDOWN in Windows API
            const int buttonDownMessage = 0xA1;

            //HTCAPTION in Windows API
            const int titleBar = 0x2;

            ReleaseCapture();
            SendMessage(windowHandle, buttonDownMessage, titleBar, 0);
        }
    }
}
