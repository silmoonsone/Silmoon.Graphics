namespace GraphicsWinFormTesting
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void Tool_Click(object sender, EventArgs e)
        {
            var form = new ToolTestForm();
            form.FormClosed += (s, args) => Close();
            form.Show();
            Hide();
        }

        private void ctlFinancialKLineButton_Click_1(object sender, EventArgs e)
        {
            var form = new FinancialChartKLineForm();
            form.FormClosed += (s, args) => Close();
            form.Show();
            Hide();
        }
    }
}
