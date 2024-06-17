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
                MessageBox.Show("An error has occurred in OnKeyPressed.", "Error Occurred");
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
                MessageBox.Show("An error has occurred in OnMouseClicked.", "Error Occurred");
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
                const int ellipseWidth = 10;

                if (_state.ClickedPoints.Count == 0)
                {
                    this.UIThread(() =>
                    {
                        MessageBox.Show("There are no clicks to save as a heatmap.", "No Clicks");
                    });
                    return;
                }

                var maxX = _state.ClickedPoints.Keys.Max(key => key.X);
                var maxY = _state.ClickedPoints.Keys.Max(key => key.Y);

                var bitmap = new Bitmap(maxX + ellipseWidth + 1, maxY + ellipseWidth + 1);

                using (var graphics = Graphics.FromImage(bitmap))
                {
                    graphics.FillRectangle(Brushes.White, new Rectangle(0, 0, bitmap.Width, bitmap.Height));

                    foreach (var point in  _state.ClickedPoints.Keys) 
                    {
                        var rectangle = new Rectangle(point.X, point.Y, ellipseWidth, ellipseWidth);

                        if (_state.ClickedPoints.ContainsKey(point))
                        {
                            var clickCount = _state.ClickedPoints[point];
                            graphics.FillEllipse(GetBrushColorFromClickCount(clickCount), rectangle);
                        }
                    }
                }

                if (!Directory.Exists(Directory.GetCurrentDirectory() + "\\images"))
                { 
                    Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\images");
                }

                bitmap.Save($"{Directory.GetCurrentDirectory()}\\images\\{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.bmp", ImageFormat.Bmp);
            }
            catch (Exception ex)
            {
                this.UIThread(() =>
                {
                    MessageBox.Show("An error occurred while saving the heatmap.", "Error Occurred");
                });
            }
            finally
            {
                this.UIThread(() =>
                {
                    _state.IsLoading = false;
                    btnSaveHeatmap.Enabled = true;
                    btnSaveHeatmap.Text = "Save Heatmap";
                });
            }
        }

        private System.Drawing.Brush GetBrushColorFromClickCount(int clickCount)
        {
            if (clickCount >= 128)
            {
                return System.Drawing.Brushes.OrangeRed;
            }

            if (clickCount >= 100)
            {
                return System.Drawing.Brushes.Red;
            }

            if (clickCount >= 86)
            {
                return System.Drawing.Brushes.Firebrick;
            }

            if (clickCount >= 64)
            {
                return System.Drawing.Brushes.DeepPink;
            }

            if (clickCount >= 48)
            {
                return System.Drawing.Brushes.Fuchsia;
            }

            if (clickCount >= 32)
            {
                return System.Drawing.Brushes.DarkViolet;
            }

            if (clickCount >= 16)
            { 
                return System.Drawing.Brushes.DarkMagenta;
            }

            if (clickCount >= 8)
            { 
                return System.Drawing.Brushes.DarkSlateBlue;
            }

            if (clickCount >= 4)
            { 
                return System.Drawing.Brushes.Blue;
            }

            if (clickCount >= 2)
            { 
                return System.Drawing.Brushes.CornflowerBlue;
            }

            if (clickCount >= 1)
            { 
                return System.Drawing.Brushes.SkyBlue;
            }

            return System.Drawing.Brushes.White;
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
