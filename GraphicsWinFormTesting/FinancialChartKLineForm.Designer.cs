namespace GraphicsWinFormTesting
{
    partial class FinancialChartKLineForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            pictureBox1 = new PictureBox();
            ctlRefreshButton = new Button();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pictureBox1.BackColor = SystemColors.ButtonShadow;
            pictureBox1.Location = new Point(0, 0);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(800, 409);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // ctlRefreshButton
            // 
            ctlRefreshButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            ctlRefreshButton.Location = new Point(12, 415);
            ctlRefreshButton.Name = "ctlRefreshButton";
            ctlRefreshButton.Size = new Size(75, 23);
            ctlRefreshButton.TabIndex = 1;
            ctlRefreshButton.Text = "button1";
            ctlRefreshButton.UseVisualStyleBackColor = true;
            ctlRefreshButton.Click += ctlRefreshButton_Click;
            // 
            // FinancialChartKLineForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(ctlRefreshButton);
            Controls.Add(pictureBox1);
            Name = "FinancialChartKLineForm";
            Text = "FinancialChartKLineForm";
            Load += FinancialChartKLineForm_Load;
            Resize += FinancialChartKLineForm_Resize;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private PictureBox pictureBox1;
        private Button ctlRefreshButton;
    }
}