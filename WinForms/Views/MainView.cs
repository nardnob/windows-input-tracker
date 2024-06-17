using nardnob.InputTracker.WindowsInteraction;
using nardnob.InputTracker.WinForms.Models;
using nardnob.InputTracker.WinForms.Utilities;
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
            catch (Exception)
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
            catch (Exception)
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

        private void SaveClicksHeatmap(string fileName)
        {
            try
            {
                if (_state.ClickedPoints.Count == 0)
                {
                    this.UIThread(() =>
                    {
                        MessageBox.Show("There are no clicks to save as a heatmap.", "No Clicks");
                    });
                    return;
                }

                var bitmap = HeatmapGenerator.GenerateHeatmapBitmap(_state.ClickedPoints);
                HeatmapGenerator.SaveBitmapImage(bitmap, fileName);
            }
            catch (Exception)
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

        private async void btnSaveHeatmap_Click(object sender, EventArgs e)
        {
            if (_state.IsLoading)
            {
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Bitmap Image|*.bmp";
            saveFileDialog.Title = "Save an Image File";
            saveFileDialog.ShowDialog();

            if (saveFileDialog.FileName != "")
            {
                _state.IsLoading = true;
                btnSaveHeatmap.Enabled = false;
                btnSaveHeatmap.Text = "Saving...";

                Debug.WriteLine($"Saving bmp to: {saveFileDialog.FileName}");
                await Task.Factory.StartNew(() => {
                    SaveClicksHeatmap(saveFileDialog.FileName);
                });
            }
        }

        #endregion
    }
}
