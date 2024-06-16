using nardnob.InputTracker.WindowsInteraction;

namespace nardnob.InputTracker.WinForms
{
    public partial class MainView : Form
    {
        public MainView()
        {
            InitializeComponent();
        }

        private void MainView_MouseDown(object sender, MouseEventArgs e)
        {
            Grabber.Grab(this.Handle);
        }
    }
}
