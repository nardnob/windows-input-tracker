using nardnob.InputTracker.WindowsInteraction;
using nardnob.InputTracker.WinForms.Models;
using System.Diagnostics;

namespace nardnob.InputTracker.WinForms.Views
{
    public partial class MainView : Form
    {
        #region " Private Members "

        KeyboardInterceptor? _keyListener;
        MouseInterceptor? _mouseListener;

        State _state = new();

        #endregion

        #region " Constructors "

        public MainView()
        {
            InitializeComponent();
        }

        #endregion

        #region " Private Methods "

        private void OnKeyPressed(object sender, KeyboardInterceptorEventArgs e)
        {
            var keyPressed = e.KeyPressed;

            _state.KeyCount++;

            UpdateFormValues();

            var pressedAlt1 = (Keys)keyPressed == Keys.D1 && Control.ModifierKeys == Keys.Alt;
            if (pressedAlt1)
            {
                ToggleFormVisibility();
            }
        }

        private void OnMouseClicked(object sender, InterceptMouseEventArgs e)
        {
            try
            {
                var mouseMessage = e.MouseMessage;
                var mousePoint = e.MousePoint;

                var isMouseClick = mouseMessage == MouseMessages.WM_LBUTTONDOWN || mouseMessage == MouseMessages.WM_RBUTTONDOWN;

                if (isMouseClick)
                {
                    _state.ClickCount++;

                    UpdateFormValues();

                    Debug.WriteLine($"Clicked point: ({mousePoint.X}, {mousePoint.Y})");

                    if (_state.ClickedPoints.ContainsKey(mousePoint.X))
                    {
                        if (_state.ClickedPoints[mousePoint.X].ContainsKey(mousePoint.Y))
                        {
                            _state.ClickedPoints[mousePoint.X][mousePoint.Y]++;
                        }
                        else
                        {
                            _state.ClickedPoints[mousePoint.X].Add(mousePoint.Y, 1);
                        }
                    }
                    else
                    {
                        _state.ClickedPoints.Add(mousePoint.X, new Dictionary<int, int> { { mousePoint.Y, 1 } });
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error in OnMouseClicked.");
                MessageBox.Show("An error has occurred in OnMouseClicked.");
            }
        }

        private void ToggleFormVisibility()
        {
            _state.IsHidden = !_state.IsHidden;

            if (_state.IsHidden)
            {
                this.Hide();
            }
            else
            {
                this.Show();
            }

            UpdateFormValues();
        }

        private void UpdateFormValues()
        {
            if (!_state.IsHidden)
            {
                lblKeyCount.Text = _state.KeyCount.ToString("N0");
                lblClickCount.Text = _state.ClickCount.ToString("N0");
            }
        }

        private void InitializeKeyListener()
        {
            _keyListener = new KeyboardInterceptor();
            _keyListener.KeyPressed += OnKeyPressed;
            _keyListener.Start();
        }

        private void InitializeMouseListener()
        {
            _mouseListener = new MouseInterceptor();
            _mouseListener.MouseClicked += OnMouseClicked;
            _mouseListener.Start();
        }

        private void PositionForm()
        {
            Rectangle workingArea = Screen.GetWorkingArea(this);
            var xPosition = workingArea.Right - Size.Width;
            var yPosition = workingArea.Bottom - Size.Height;

            this.Location = new Point(xPosition, yPosition);
        }

        #endregion

        #region " Event Handlers "

        private void MainView_Load(object sender, EventArgs e)
        {
            PositionForm();
            InitializeKeyListener();
            InitializeMouseListener();
        }

        private void MainView_FormClosing(object sender, FormClosingEventArgs e)
        {
            _keyListener?.Stop();
            _mouseListener?.Stop();
        }

        private void MainView_MouseDown(object sender, MouseEventArgs e)
        {
            WindowGrabber.Grab(this.Handle);
        }

        #endregion
    }
}
