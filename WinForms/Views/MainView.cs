using nardnob.InputTracker.WindowsInteraction;
using nardnob.InputTracker.WinForms.Models;
using nardnob.InputTracker.WinForms.Utilities;
using System.Diagnostics;
using System.Drawing.Imaging;

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

            var clickedPoint = new Point(x, y);
            if (_state.ClickedPoints.ContainsKey(clickedPoint))
            {
                _state.ClickedPoints[clickedPoint]++;
            }
            else
            {
                _state.ClickedPoints.Add(clickedPoint, 1);
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

        private void SaveClicksHeatmap()
        {
            try
            {
                if (_state.ClickedPoints.Count == 0)
                {
                    return;
                }

                var maxX = _state.ClickedPoints.Keys.Max(key => key.X);
                var maxY = _state.ClickedPoints.Keys.Max(key => key.Y);

                var bitmap = new Bitmap(maxX, maxY);

                using (var graphics = Graphics.FromImage(bitmap))
                {
                    foreach (var point in _state.ClickedPoints.Keys)
                    {
                        var rect = new System.Drawing.Rectangle(point.X, point.Y, 1, 1);
                        var clickCount = _state.ClickedPoints[point];

                        graphics.FillRectangle(System.Drawing.Brushes.Red, rect);
                    }

                    /*
                    for (int x = 0; x < maxKeyX; x++)
                    {
                        for (int y = 0; y < maxKeyY; y++)
                        {
                            var rect = new System.Drawing.Rectangle(x, y, 1, 1);
                            if (_state.ClickedPoints.ContainsKey(x) && _state.ClickedPoints[x].ContainsKey(y))
                            { 
                                graphics.FillRectangle(GetColorFromClickCount(_state.ClickedPoints[x][y]), rect);
                            }
                            else
                            { 
                                graphics.FillRectangle(System.Drawing.Brushes.White, rect);
                            }
                        }
                    }
                    */
                }

                if (!Directory.Exists(Directory.GetCurrentDirectory() + "\\images"))
                { 
                    Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\images");
                }

                bitmap.Save(String.Format("{0}\\images\\{1}.jpg", Directory.GetCurrentDirectory(), DateTime.Now.ToString("yyyyMMddHHmmssfff")), ImageFormat.Bmp);
            }
            catch (Exception ex)
            {

            }
            finally
            {
                _state.IsLoading = false;
                this.UIThread(() =>
                {
                    btnSaveHeatmap.Enabled = true;
                    btnSaveHeatmap.Text = "Save Heatmap";
                });
            }
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

        private void btnSaveHeatmap_Click(object sender, EventArgs e)
        {
            if (_state.IsLoading)
            {
                return;
            }

            _state.IsLoading = true;
            btnSaveHeatmap.Enabled = false;
            btnSaveHeatmap.Text = "Saving...";
            Task.Factory.StartNew(SaveClicksHeatmap);
        }

        #endregion
    }
}
