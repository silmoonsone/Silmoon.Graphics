namespace GraphicsWinFormTesting
{
    partial class MainForm
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
            Tool = new Button();
            ctlFinancialKLineButton = new Button();
            SuspendLayout();
            // 
            // Tool
            // 
            Tool.Location = new Point(54, 33);
            Tool.Name = "Tool";
            Tool.Size = new Size(75, 23);
            Tool.TabIndex = 0;
            Tool.Text = "Tool";
            Tool.UseVisualStyleBackColor = true;
            Tool.Click += Tool_Click;
            // 
            // ctlFinancialKLineButton
            // 
            ctlFinancialKLineButton.Location = new Point(54, 108);
            ctlFinancialKLineButton.Name = "ctlFinancialKLineButton";
            ctlFinancialKLineButton.Size = new Size(75, 23);
            ctlFinancialKLineButton.TabIndex = 1;
            ctlFinancialKLineButton.Text = "金融K线图";
            ctlFinancialKLineButton.UseVisualStyleBackColor = true;
            ctlFinancialKLineButton.Click += ctlFinancialKLineButton_Click_1;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(ctlFinancialKLineButton);
            Controls.Add(Tool);
            Name = "MainForm";
            Text = "Form1";
            ResumeLayout(false);
        }

        #endregion

        private Button Tool;
        private Button ctlFinancialKLineButton;
    }
}
