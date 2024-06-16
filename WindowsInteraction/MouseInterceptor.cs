using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;

namespace nardnob.InputTracker.WindowsInteraction
{
    public class MouseInterceptor
    {
        #region " Public Members "

        public event EventHandler<InterceptMouseEventArgs>? MouseClicked;

        #endregion

        #region " Private Members "

        private const int WH_MOUSE_LL = 14;
        private static IntPtr _hookID = IntPtr.Zero;

        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);
        private LowLevelMouseProc? _hookCallback;

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MSLLHOOKSTRUCT
        {
            public POINT pt;
            public uint mouseData;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        #endregion

        #region " DLL Imports "

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        #endregion

        #region " Public Methods "

        public void Start()
        {
            if (_hookID != IntPtr.Zero)
            { 
                UnhookWindowsHookEx(_hookID);
            }

            _hookCallback = new LowLevelMouseProc(HookCallback);
            _hookID = SetHook(_hookCallback);
        }

        public void Stop()
        {
            UnhookWindowsHookEx(_hookID);
            _hookID = IntPtr.Zero;
        }

        #endregion

        #region " Private Methods "

        private static IntPtr SetHook(LowLevelMouseProc proc)
        {
            using (Process currentProcess = Process.GetCurrentProcess())
            using (ProcessModule currentModule = currentProcess.MainModule)
            {
                if (currentModule is null)
                {
                    throw new Exception("currentModule should not be null here.");
                }

                return SetWindowsHookEx(WH_MOUSE_LL, proc, GetModuleHandle(currentModule.ModuleName), 0);
            }
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
                if (MouseClicked is not null)
                {
                    var interceptMouseEventArgs = new InterceptMouseEventArgs((MouseMessages)wParam, new Point(hookStruct.pt.x, hookStruct.pt.y));
                    MouseClicked.Invoke(this, interceptMouseEventArgs);
                }
            }

            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        #endregion
    }
}
