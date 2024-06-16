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
            try
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
            catch (Exception ex)
            {
                Debug.WriteLine("Error in OnKeyPressed.");
                MessageBox.Show("An error has occurred in OnKeyPressed.");
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
                    StoreClickedPoint(mousePoint.X, mousePoint.Y, mouseMessage);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error in OnMouseClicked.");
                MessageBox.Show("An error has occurred in OnMouseClicked.");
            }
        }

        private void StoreClickedPoint(int x, int y, MouseMessages mouseMessage = MouseMessages.WM_LBUTTONDOWN)
        {
            var isLeftClick = mouseMessage == MouseMessages.WM_LBUTTONDOWN;
            var theClickedButton = isLeftClick ? "Left" : "Right";

            Debug.WriteLine($"{theClickedButton}-clicked point: ({x}, {y})");

            if (_state.ClickedPoints.ContainsKey(new Point(x, y)))
            {
                _state.ClickedPoints[new Point(x, y)]++;
            }
            else
            {
                _state.ClickedPoints.Add(new Point(x, y), 1);
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
