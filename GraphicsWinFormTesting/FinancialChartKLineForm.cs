using Silmoon.Graphics.Financial;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GraphicsWinFormTesting
{
    public partial class FinancialChartKLineForm : Form
    {
        private ActionableCandleChart _chart;
        public FinancialChartKLineForm()
        {
            InitializeComponent();
            _chart = new ActionableCandleChart(1, pictureBox1.Width, pictureBox1.Height);
            _chart.FrameRefreshed(frame =>
            {
                using var image = SKImage.FromBitmap(_chart.Bitmap);
                using var data = image.Encode(SKEncodedImageFormat.Png, 100);
                var stream = data.AsStream();
                var bitmap = new Bitmap(stream);
                Invoke(() =>
                {
                    if (!IsDisposed)
                    {
                        pictureBox1.Image?.Dispose();
                        pictureBox1.Image = bitmap;
                    }
                });
            });
        }
        private void UpdateChartSize()
        {
            int width = pictureBox1.Width;
            int height = pictureBox1.Height;

            // 确保尺寸有效
            if (width > 0 && height > 0)
            {
                _chart.SetSize(width, height);
            }
        }

        private void RefreshChart()
        {
            _chart.RefreshFrame();
        }

        private void FinancialChartKLineForm_Load(object sender, EventArgs e)
        {
            RefreshChart();
        }

        private void ctlRefreshButton_Click(object sender, EventArgs e)
        {
            RefreshChart();
        }

        private void FinancialChartKLineForm_Resize(object sender, EventArgs e)
        {
            UpdateChartSize();
            RefreshChart();
        }
    }
}
