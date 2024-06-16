using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;

namespace nardnob.InputTracker.WindowsInteraction
{
    public class InterceptMouseEventArgs : HandledEventArgs
    {
        public MouseMessages MouseMessage { get; private set; }
        public Point MousePoint { get; private set; }

        public InterceptMouseEventArgs(MouseMessages mouseMessage, Point point)
        {
            MouseMessage = mouseMessage;
            MousePoint = point;
        }
    }
}
