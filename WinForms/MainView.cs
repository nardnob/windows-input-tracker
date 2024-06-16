using nardnob.InputTracker.WindowsInteraction;
using static System.Windows.Forms.AxHost;

namespace nardnob.InputTracker.WinForms
{
    public partial class MainView : Form
    {
        #region " Private Members "

        KeyboardInterceptor? _keyListener;
        State _state = new();

        #endregion

        public MainView()
        {
            InitializeComponent();
        }

        #region " Private Methods "

        private void OnKeyPressed(object sender, KeyboardInterceptorEventArgs e)
        {
            var keyPressed = e.KeyPressed;

            _state.KeyCount++;

            if (!_state.IsHidden)
            {
                UpdateFormValues();
            }

            if ((Keys)keyPressed == Keys.D1 && Control.ModifierKeys == Keys.Alt)
            {
                ToggleFormVisibility();
            }
        }

        #endregion

        #region " Private Methods "

        private void ToggleFormVisibility()
        {
            _state.IsHidden = !_state.IsHidden;

            if (_state.IsHidden)
            {
                this.Hide();
            }
            else
            {
                UpdateFormValues();
                this.Show();
            }
        }

        private void UpdateFormValues()
        {
            lblKeyCount.Text = _state.KeyCount.ToString("N0");
        }

        #endregion

        #region " Event Handlers "

        private void MainView_Load(object sender, EventArgs e)
        {

            _keyListener = new KeyboardInterceptor();
            _keyListener.KeyPressed += OnKeyPressed;
            _keyListener.Start();
        }

        private void MainView_MouseDown(object sender, MouseEventArgs e)
        {
            Grabber.Grab(this.Handle);
        }

        private void lblKeyCountDescription_MouseDown(object sender, MouseEventArgs e)
        {
            Grabber.Grab(this.Handle);
        }

        private void lblKeyCount_MouseDown(object sender, MouseEventArgs e)
        {
            Grabber.Grab(this.Handle);
        }

        #endregion
    }
}
