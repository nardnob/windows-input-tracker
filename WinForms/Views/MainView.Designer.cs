namespace nardnob.InputTracker.WinForms.Views
{
    partial class MainView
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lblKeyCount = new Label();
            lblKeyCountDescription = new Label();
            lblToggleVisibilityDescription = new Label();
            lblClickCountDescription = new Label();
            lblClickCount = new Label();
            SuspendLayout();
            // 
            // lblKeyCount
            // 
            lblKeyCount.AutoSize = true;
            lblKeyCount.Location = new Point(83, 9);
            lblKeyCount.Name = "lblKeyCount";
            lblKeyCount.Size = new Size(13, 15);
            lblKeyCount.TabIndex = 0;
            lblKeyCount.Text = "0";
            lblKeyCount.MouseDown += MainView_MouseDown;
            // 
            // lblKeyCountDescription
            // 
            lblKeyCountDescription.AutoSize = true;
            lblKeyCountDescription.Location = new Point(12, 9);
            lblKeyCountDescription.Name = "lblKeyCountDescription";
            lblKeyCountDescription.Size = new Size(65, 15);
            lblKeyCountDescription.TabIndex = 1;
            lblKeyCountDescription.Text = "Key Count:";
            lblKeyCountDescription.MouseDown += MainView_MouseDown;
            // 
            // lblToggleVisibilityDescription
            // 
            lblToggleVisibilityDescription.AutoSize = true;
            lblToggleVisibilityDescription.Location = new Point(462, 426);
            lblToggleVisibilityDescription.Name = "lblToggleVisibilityDescription";
            lblToggleVisibilityDescription.Size = new Size(326, 15);
            lblToggleVisibilityDescription.TabIndex = 2;
            lblToggleVisibilityDescription.Text = "(ALT+1) - Hold ALT and press 1 to toggle the form's visibility.";
            lblToggleVisibilityDescription.MouseDown += MainView_MouseDown;
            // 
            // lblClickCountDescription
            // 
            lblClickCountDescription.AutoSize = true;
            lblClickCountDescription.Location = new Point(12, 33);
            lblClickCountDescription.Name = "lblClickCountDescription";
            lblClickCountDescription.Size = new Size(72, 15);
            lblClickCountDescription.TabIndex = 3;
            lblClickCountDescription.Text = "Click Count:";
            lblClickCountDescription.MouseDown += MainView_MouseDown;
            // 
            // lblClickCount
            // 
            lblClickCount.AutoSize = true;
            lblClickCount.Location = new Point(90, 33);
            lblClickCount.Name = "lblClickCount";
            lblClickCount.Size = new Size(13, 15);
            lblClickCount.TabIndex = 4;
            lblClickCount.Text = "0";
            lblClickCount.MouseDown += MainView_MouseDown;
            // 
            // MainView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(lblClickCount);
            Controls.Add(lblClickCountDescription);
            Controls.Add(lblToggleVisibilityDescription);
            Controls.Add(lblKeyCountDescription);
            Controls.Add(lblKeyCount);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "MainView";
            Text = "Windows Input Tracker";
            TopMost = true;
            FormClosing += MainView_FormClosing;
            Load += MainView_Load;
            MouseDown += MainView_MouseDown;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblKeyCount;
        private Label lblKeyCountDescription;
        private Label lblToggleVisibilityDescription;
        private Label lblClickCountDescription;
        private Label lblClickCount;
    }
}
