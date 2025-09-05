using Silmoon.Extension;
using Silmoon.Graphics.Extension;
using SkiaSharp;

namespace GraphicsWinFormTesting
{
    public partial class ToolTestForm : Form
    {
        public ToolTestForm()
        {
            InitializeComponent();
        }

        private void ctlResizeTestButton_Click(object sender, EventArgs e)
        {
            using SKImage image = File.ReadAllBytes(ctlFilePathTextBox.Text).GetSKImage();
            using SKBitmap bitmap = image.ToSKBitmap();
            using SKBitmap resizedBitmap = bitmap.Resize(1000, 1000, false, true);

            using var stream = resizedBitmap.GetBytes().GetStream();
            ctlImagePictureBox.Image = Image.FromStream(stream);
        }

        private void ctlSelectFileButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image Files(*.BMP;*.JPG;*.GIF;*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG|All files (*.*)|*.*";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                ctlFilePathTextBox.Text = ofd.FileName;
                using var stream = File.ReadAllBytes(ofd.FileName).GetSKImage().GetBytes().GetStream();
                ctlImagePictureBox.Image = Image.FromStream(stream);
            }
        }

        private void ctlCompressTestButton_Click(object sender, EventArgs e)
        {
            var data = File.ReadAllBytes(ctlFilePathTextBox.Text);
            MessageBox.Show(data.Length.ToString());
            using SKImage image = data.GetSKImage();
            var compressedImage = image.Compress(null, 10);
            data = compressedImage.GetBytes();
            MessageBox.Show(data.Length.ToString());

            using var stream = compressedImage.GetBytes().GetStream();
            ctlImagePictureBox.Image = Image.FromStream(stream);
        }
    }
}
