using Silmoon.Extension;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace Silmoon.Graphics.Extension
{
    public static class SKImageExtension
    {
        // 压缩 SKImage
        public static SKImage Compress(this SKImage image, SKEncodedImageFormat? format = null, int qualityLevel = 80)
        {
            var targetFormat = format ?? DetectImageFormat(image) ?? SKEncodedImageFormat.Jpeg;
            using var data = image.Encode(targetFormat, qualityLevel);
            return SKImage.FromEncodedData(data);
        }
        // 压缩 byte[] 数据
        public static byte[] Compress(byte[] imageData, SKEncodedImageFormat? format = null, int qualityLevel = 80)
        {
            using var image = SKImage.FromEncodedData(imageData);

            var targetFormat = format ?? DetectImageFormat(image) ?? SKEncodedImageFormat.Jpeg;
            using var data = image.Encode(targetFormat, qualityLevel);
            return data.ToArray();
        }

        // 获取 SKImage 的二进制数组
        public static byte[] GetBytes(this SKImage image)
        {
            using var data = image.Encode();
            return data.ToArray();
        }

        // 从 byte[] 创建 SKImage
        public static SKImage GetSKImage(this byte[] imageData)
        {
            return SKImage.FromEncodedData(imageData);
        }

        // 将 SKImage 转换为 SKBitmap
        public static SKBitmap ToSKBitmap(this SKImage image)
        {
            return SKBitmap.FromImage(image);
        }


        // 修复 iPhone 照片方向
        public static SKImage FixiPhoneOrientation(this SKImage image)
        {
            var orientation = image.GetOrientation();
            if (orientation == SKEncodedOrigin.TopLeft) return image;

            using var bitmap = image.ToSKBitmap();
            using var rotatedBitmap = orientation switch
            {
                SKEncodedOrigin.RightTop => bitmap.Rotate(90),
                SKEncodedOrigin.BottomRight => bitmap.Rotate(180),
                SKEncodedOrigin.LeftBottom => bitmap.Rotate(270),
                _ => bitmap
            };
            return SKImage.FromBitmap(rotatedBitmap);
        }
        // 辅助方法：获取方向
        public static SKEncodedOrigin GetOrientation(this SKImage image)
        {
            using var data = image.Encode();
            using var codec = SKCodec.Create(data);
            return codec?.EncodedOrigin ?? SKEncodedOrigin.TopLeft;
        }


        // 检测图像的原始格式
        public static SKEncodedImageFormat? DetectImageFormat(SKImage image)
        {
            using var data = image.Encode();
            using var codec = SKCodec.Create(data);
            return codec?.EncodedFormat;
        }
    }
}
