using Silmoon.Extension;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace Silmoon.Graphics.Extension
{
    public static class SKBitmapExtension
    {
        // 调整大小
        public static SKBitmap Resize(this SKBitmap bitmap, int width, int height)
        {
            var resizedBitmap = new SKBitmap(width, height);
            using var canvas = new SKCanvas(resizedBitmap);
            using var paint = new SKPaint
            {
                FilterQuality = SKFilterQuality.High,
                IsAntialias = true
            };
            canvas.DrawBitmap(bitmap, new SKRect(0, 0, width, height), paint);
            return resizedBitmap;
        }
        // 调整大小
        public static SKBitmap Resize(this SKBitmap bitmap, int width, int height, bool ifTargetSizeGreaterKeepOriginal, bool maintainAspectRatio)
        {
            // 如果设置了保持原尺寸且目标尺寸大于当前尺寸，直接返回原图
            if (ifTargetSizeGreaterKeepOriginal && (width > bitmap.Width || height > bitmap.Height)) return bitmap;

            // 根据保持比例选项调整宽高
            if (maintainAspectRatio)
            {
                // 计算原始宽高比
                float aspectRatio = (float)bitmap.Width / bitmap.Height;

                // 调整目标宽高以保持比例
                if (width > 0 && height == 0)
                    height = (int)(width / aspectRatio);
                else if (height > 0 && width == 0)
                    width = (int)(height * aspectRatio);
                else
                {
                    if ((float)width / height > aspectRatio)
                        width = (int)(height * aspectRatio);
                    else
                        height = (int)(width / aspectRatio);
                }
            }

            // 创建目标大小的 SKBitmap
            var resizedBitmap = new SKBitmap(width, height);
            using var canvas = new SKCanvas(resizedBitmap);
            using var paint = new SKPaint
            {
                FilterQuality = SKFilterQuality.High,
                IsAntialias = true
            };
            canvas.DrawBitmap(bitmap, new SKRect(0, 0, width, height), paint);
            return resizedBitmap;
        }

        // 调整宽度
        public static SKBitmap ResizeWidth(this SKBitmap bitmap, int width, bool ifTargetSizeGreaterKeepOriginal, bool maintainAspectRatio = false)
        {
            if (ifTargetSizeGreaterKeepOriginal && bitmap.Width < width) width = bitmap.Width;

            double scaleFactor = (double)width / bitmap.Width;
            int height = bitmap.Height;
            if (maintainAspectRatio) height = (int)(bitmap.Height * scaleFactor);

            return bitmap.Resize(width, height);
        }
        // 调整高度
        public static SKBitmap ResizeHeight(this SKBitmap bitmap, int height, bool ifTargetSizeGreaterKeepOriginal, bool maintainAspectRatio = false)
        {
            if (ifTargetSizeGreaterKeepOriginal && bitmap.Height < height) height = bitmap.Height;

            double scaleFactor = (double)height / bitmap.Height;
            int width = bitmap.Width;
            if (maintainAspectRatio) width = (int)(bitmap.Width * scaleFactor);

            return bitmap.Resize(width, height);
        }


        // 获取 SKBitmap 的字节数组
        public static byte[] GetBytes(this SKBitmap bitmap)
        {
            using var image = SKImage.FromBitmap(bitmap);
            return image.GetBytes();
        }

        // 从 byte[] 创建 SKBitmap
        public static SKBitmap FromBytes(byte[] imageData) => SKBitmap.Decode(imageData);

        // 将 SKBitmap 转换为 SKImage
        public static SKImage ToSKImage(this SKBitmap bitmap) => SKImage.FromBitmap(bitmap);

        // 辅助方法：旋转
        public static SKBitmap Rotate(this SKBitmap bitmap, float degrees)
        {
            var rotatedBitmap = new SKBitmap(bitmap.Height, bitmap.Width);
            using var canvas = new SKCanvas(rotatedBitmap);
            canvas.Translate(rotatedBitmap.Width / 2f, rotatedBitmap.Height / 2f);
            canvas.RotateDegrees(degrees);
            canvas.DrawBitmap(bitmap, -bitmap.Width / 2f, -bitmap.Height / 2f);
            return rotatedBitmap;
        }
    }
}
