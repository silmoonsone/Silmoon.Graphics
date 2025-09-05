namespace GraphicsWinFormTesting
{
    partial class ToolTestForm
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
            ctlImagePictureBox = new PictureBox();
            ctlResizeTestButton = new Button();
            ctlFilePathTextBox = new TextBox();
            ctlSelectFileButton = new Button();
            ctlCompressTestButton = new Button();
            ((System.ComponentModel.ISupportInitialize)ctlImagePictureBox).BeginInit();
            SuspendLayout();
            // 
            // ctlImagePictureBox
            // 
            ctlImagePictureBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            ctlImagePictureBox.Location = new Point(12, 41);
            ctlImagePictureBox.Name = "ctlImagePictureBox";
            ctlImagePictureBox.Size = new Size(624, 397);
            ctlImagePictureBox.TabIndex = 0;
            ctlImagePictureBox.TabStop = false;
            // 
            // ctlResizeTestButton
            // 
            ctlResizeTestButton.Location = new Point(642, 12);
            ctlResizeTestButton.Name = "ctlResizeTestButton";
            ctlResizeTestButton.Size = new Size(146, 23);
            ctlResizeTestButton.TabIndex = 2;
            ctlResizeTestButton.Text = "ResizeTest";
            ctlResizeTestButton.UseVisualStyleBackColor = true;
            ctlResizeTestButton.Click += ctlResizeTestButton_Click;
            // 
            // ctlFilePathTextBox
            // 
            ctlFilePathTextBox.Location = new Point(12, 12);
            ctlFilePathTextBox.Name = "ctlFilePathTextBox";
            ctlFilePathTextBox.Size = new Size(543, 23);
            ctlFilePathTextBox.TabIndex = 0;
            // 
            // ctlSelectFileButton
            // 
            ctlSelectFileButton.Location = new Point(561, 12);
            ctlSelectFileButton.Name = "ctlSelectFileButton";
            ctlSelectFileButton.Size = new Size(75, 23);
            ctlSelectFileButton.TabIndex = 1;
            ctlSelectFileButton.Text = "...(&B)";
            ctlSelectFileButton.UseVisualStyleBackColor = true;
            ctlSelectFileButton.Click += ctlSelectFileButton_Click;
            // 
            // ctlCompressTestButton
            // 
            ctlCompressTestButton.Location = new Point(642, 41);
            ctlCompressTestButton.Name = "ctlCompressTestButton";
            ctlCompressTestButton.Size = new Size(146, 23);
            ctlCompressTestButton.TabIndex = 3;
            ctlCompressTestButton.Text = "CompressTest";
            ctlCompressTestButton.UseVisualStyleBackColor = true;
            ctlCompressTestButton.Click += ctlCompressTestButton_Click;
            // 
            // ToolTestForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(ctlCompressTestButton);
            Controls.Add(ctlSelectFileButton);
            Controls.Add(ctlFilePathTextBox);
            Controls.Add(ctlResizeTestButton);
            Controls.Add(ctlImagePictureBox);
            Name = "ToolTestForm";
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)ctlImagePictureBox).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox ctlImagePictureBox;
        private Button ctlResizeTestButton;
        private TextBox ctlFilePathTextBox;
        private Button ctlSelectFileButton;
        private Button ctlCompressTestButton;
    }
}
