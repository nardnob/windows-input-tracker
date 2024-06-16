using System.Diagnostics;
using System.Runtime.InteropServices;

namespace nardnob.InputTracker.WindowsInteraction
{
    public class KeyboardInterceptor
    {
        public event EventHandler<KeyboardInterceptorEventArgs>? KeyPressed;

        #region " Private Members "

        private LowLevelKeyboardProc? _hookCallback;
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_SYSKEYDOWN = 0x0104;
        private IntPtr? _hookID = IntPtr.Zero;

        #endregion

        #region " DLL Imports "

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

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
            if (_hookID is null)
            {
                throw new Exception("_hookID should not be null at this point.");
            }

            if (_hookID != IntPtr.Zero)
            { 
                UnhookWindowsHookEx((IntPtr)_hookID);
            }

            _hookCallback = new LowLevelKeyboardProc(HookCallback);
            _hookID = SetHook(_hookCallback);
        }

        public void Stop()
        {
            if ( _hookID is null)
            {
                throw new Exception("_hookID should not be null at this point.");
            }

            UnhookWindowsHookEx((IntPtr)_hookID);
            _hookID = IntPtr.Zero;
        }

        #endregion

        #region " Private Methods "

        private static IntPtr? SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule? curModule = curProcess.MainModule)
            {
                if (curModule is null)
                {
                    return null;
                }

                return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (_hookID is null)
            {
                throw new Exception("_hookID should not be null at this point.");
            }

            if (nCode >= 0 && (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN))
            {
                int vkCode = Marshal.ReadInt32(lParam);
                if (KeyPressed != null)
                { 
                    KeyPressed.Invoke(this, new KeyboardInterceptorEventArgs(vkCode));
                }
            }

            return CallNextHookEx((IntPtr)_hookID, nCode, wParam, lParam);
        }

        #endregion
    }
}
