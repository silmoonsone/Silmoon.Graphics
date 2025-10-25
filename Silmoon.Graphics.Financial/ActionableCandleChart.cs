using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Silmoon.Graphics.Financial
{
    public class ActionableCandleChart : IDisposable
    {
        Action<SKBitmap> OnFrameRefreshed;
        public SKBitmap Bitmap { get; private set; }
        public int Width { get; set; } = 800;
        public int Height { get; set; } = 600;
        public double CurrentFps { get; set; } = 0.0;
        public double Fps { get; set; } = 1.0;
        Timer timer;
        // 帧率计算相关
        DateTime _lastFrameTime = DateTime.Now;
        int _frameCount = 0;
        object _fpsLock = new object();
        object _drawingLock = false;

        SKFont textFont;
        SKFont fpsFont;

        SKPaint textPaint;
        SKPaint fpsPaint;


        public ActionableCandleChart() => init();
        public ActionableCandleChart(double frameFps = 1, int width = 800, int height = 600) => init(frameFps, width, height);
        public void init(double frameFps = 1, int width = 800, int height = 600)
        {
            Fps = frameFps;
            Width = width;
            Height = height;

            Bitmap = new SKBitmap(Width, Height);

            textFont = new SKFont() { Size = 24, Typeface = SKTypeface.Default };
            fpsFont = new SKFont() { Size = 16, Typeface = SKTypeface.Default };
            textPaint = new SKPaint { Color = SKColors.White, IsAntialias = true, };
            fpsPaint = new SKPaint { Color = SKColors.Red, IsAntialias = true, };

            timer = new Timer(RefreshFrame, null, 0, (int)(1000 / Fps));
        }



        /// <summary>
        /// 设置图表尺寸
        /// </summary>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        public void SetSize(int width, int height)
        {
            Width = width;
            Height = height;
            Bitmap?.Dispose();
            Bitmap = new SKBitmap(Width, Height);
        }

        /// <summary>
        /// 计算帧率
        /// </summary>
        private void CalculateFps()
        {
            lock (_fpsLock)
            {
                _frameCount++;
                var now = DateTime.Now;
                var elapsed = (now - _lastFrameTime).TotalSeconds;

                // 每秒更新一次帧率
                if (elapsed >= 1.0)
                {
                    CurrentFps = _frameCount / elapsed;
                    _frameCount = 0;
                    _lastFrameTime = now;
                }
            }
        }

        void RefreshFrame(object state)
        {
            RefreshFrame();
        }
        public SKBitmap RefreshFrame()
        {
            lock (_drawingLock)
            {
                if ((bool)_drawingLock) return Bitmap;
                _drawingLock = true;
                using var canvas = new SKCanvas(Bitmap);
                canvas.Clear(SKColors.Black);

                // 获取当前时间
                string currentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                // 计算文本位置（居中）
                var textBounds = new SKRect();
                textFont.MeasureText(currentTime, out textBounds);

                // 计算水平居中位置
                float x = (Width - textBounds.Width) / 2;
                // 计算垂直居中位置
                float y = (Height - textBounds.Height) / 2 + textBounds.Height;

                // 绘制时间文本
                canvas.DrawText(currentTime, x, y, SKTextAlign.Left, textFont, textPaint);

                // 绘制帧率信息（左上角）
                CalculateFps();
                DrawFps(canvas);

                OnFrameRefreshed?.Invoke(Bitmap);
                _drawingLock = false;
                return Bitmap;
            }
        }

        /// <summary>
        /// 绘制帧率信息
        /// </summary>
        private void DrawFps(SKCanvas canvas)
        {
            // 获取当前帧率
            double fps = CurrentFps;
            string fpsText = $"FPS: {fps:F1}";

            // 在左上角绘制帧率
            canvas.DrawText(fpsText, 10, 25, SKTextAlign.Left, fpsFont, fpsPaint);
        }

        public void FrameRefreshed(Action<SKBitmap> action) => OnFrameRefreshed = action;
        public void Dispose()
        {
            timer?.Dispose();

            textFont?.Dispose();
            fpsFont?.Dispose();
            textPaint?.Dispose();
            fpsPaint?.Dispose();

            Bitmap?.Dispose();
        }
    }
}
