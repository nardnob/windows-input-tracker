namespace nardnob.InputTracker.WinForms.Utilities
{
    internal static class Extensions
    {
        public static void UIThread(this Control control, Action code)
        {
            if (control.IsDisposed)
            { 
                return;
            }

            if (control.InvokeRequired)
            {
                control.BeginInvoke(code);
            }
            else
            {
                code();
            }
        }
    }
}
