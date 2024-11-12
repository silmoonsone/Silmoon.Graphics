using Silmoon.Extension;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace Silmoon.Drawing
{
    public static class SKImageExtension
    {
        // 压缩 SKImage
        public static byte[] Compress(this SKImage image, SKEncodedImageFormat? format = null, int qualityLevel = 80)
        {
            var targetFormat = format ?? DetectImageFormat(image) ?? SKEncodedImageFormat.Jpeg;
            using (var data = image.Encode(targetFormat, qualityLevel))
            {
                return data.ToArray();
            }
        }
        // 压缩 byte[] 数据
        public static byte[] Compress(byte[] imageData, SKEncodedImageFormat? format = null, int qualityLevel = 80)
        {
            using (var image = SKImage.FromEncodedData(imageData))
            {
                return Compress(image, format, qualityLevel);
            }
        }

        // 获取 SKImage 的二进制数组
        public static byte[] GetBytes(this SKImage image, SKEncodedImageFormat imageFormat = SKEncodedImageFormat.Png, int quality = 100)
        {
            using (var data = image.Encode(imageFormat, quality))
            {
                return data.ToArray();
            }
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

        // 检测图像的原始格式
        public static SKEncodedImageFormat? DetectImageFormat(SKImage image)
        {
            // 将 SKImage 转换为 SKData 以便解码
            using var data = image.Encode();
            // 使用 SKCodec 从数据中解码格式
            using var codec = SKCodec.Create(data);
            // 如果 codec 成功创建，返回编码格式
            return codec?.EncodedFormat;
        }
    }
}
