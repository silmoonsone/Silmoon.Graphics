using Silmoon.Extension;
using Silmoon.Graphics.Extension;
using SkiaSharp;

namespace WinFormTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void ctlResizeTestButton_Click(object sender, EventArgs e)
        {
            SKImage image = File.ReadAllBytes(ctlFilePathTextBox.Text).GetSKImage();
            SKBitmap bitmap = image.ToSKBitmap();
            SKBitmap resizedBitmap = bitmap.Resize(1000, 1000, true, true);

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
            SKImage image = data.GetSKImage();
            data = image.Compress(null, 10);
            MessageBox.Show(data.Length.ToString());

            var compressedImage = data.GetSKImage();
            using var stream = compressedImage.GetBytes().GetStream();
            ctlImagePictureBox.Image = Image.FromStream(stream);
        }
    }
}
