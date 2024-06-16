namespace nardnob.InputTracker.WindowsInteraction
{
    public class KeyboardInterceptorEventArgs
    {
        public int KeyPressed { get; private set; }

        public KeyboardInterceptorEventArgs(int keyPressed)
        {
            KeyPressed = keyPressed;
        }
    }
}
