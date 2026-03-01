using Silmoon.Extensions;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Silmoon.Graphics.Extensions
{
    /// <summary>
    /// 提供 <see cref="SKImage"/> 的扩展方法。
    /// </summary>
    /// <remarks>
    /// 返回 <see cref="SKImage"/> 或 <see cref="SKBitmap"/> 的方法，调用方负责释放返回值，否则会造成内存泄漏。
    /// <see cref="FixiPhoneOrientation"/> 在无需旋转时可能返回与输入相同的引用，请勿重复释放。
    /// </remarks>
    public static class SKImageExtension
    {
        /// <summary>
        /// 压缩 SKImage 为指定格式。
        /// </summary>
        /// <param name="image">源图像。</param>
        /// <param name="format">目标格式，null 时自动检测。</param>
        /// <param name="qualityLevel">质量 0-100。</param>
        /// <returns>压缩后的新 SKImage，与 <paramref name="image"/> 非同一引用。调用方负责释放，否则会造成内存泄漏。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="image"/> 为 null。</exception>
        public static SKImage Compress(this SKImage image, SKEncodedImageFormat? format = null, int qualityLevel = 80)
        {
            var targetFormat = format ?? DetectImageFormat(image) ?? SKEncodedImageFormat.Jpeg;
            using var data = image.Encode(targetFormat, qualityLevel);
            return SKImage.FromEncodedData(data);
        }

        /// <summary>
        /// 压缩字节数组中的图像为指定格式。
        /// </summary>
        /// <param name="imageData">图像数据（支持 PNG、JPEG、WebP 等格式）。</param>
        /// <param name="format">目标格式，null 时自动检测。</param>
        /// <param name="qualityLevel">质量 0-100。</param>
        /// <returns>压缩后的字节数组，无需释放。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="imageData"/> 为 null。</exception>
        public static byte[] Compress(byte[] imageData, SKEncodedImageFormat? format = null, int qualityLevel = 80)
        {
            using var image = SKImage.FromEncodedData(imageData);

            var targetFormat = format ?? DetectImageFormat(image) ?? SKEncodedImageFormat.Jpeg;
            using var data = image.Encode(targetFormat, qualityLevel);
            return data.ToArray();
        }

        /// <summary>
        /// 将 SKImage 编码为字节数组（默认 PNG 格式）。
        /// </summary>
        /// <param name="image">源图像。</param>
        /// <returns>编码后的字节数组，无需释放。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="image"/> 为 null。</exception>
        public static byte[] GetBytes(this SKImage image)
        {
            using var data = image.Encode();
            return data.ToArray();
        }

        /// <summary>
        /// 从字节数组解码创建 SKImage。
        /// </summary>
        /// <param name="imageData">图像数据（支持 PNG、JPEG、WebP 等格式）。</param>
        /// <returns>解码后的 SKImage，与 <paramref name="imageData"/> 无引用关系。调用方负责释放，否则会造成内存泄漏。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="imageData"/> 为 null。</exception>
        public static SKImage GetSKImage(this byte[] imageData)
        {
            return SKImage.FromEncodedData(imageData);
        }

        /// <summary>
        /// 将 SKImage 转换为 SKBitmap。
        /// </summary>
        /// <param name="image">源图像。</param>
        /// <returns>新的 SKBitmap，与 <paramref name="image"/> 非同一引用。调用方负责释放，否则会造成内存泄漏。</returns>
        /// <remarks>
        /// <para>返回的 SKBitmap 与 <paramref name="image"/> 共享像素，<paramref name="image"/> 在 SKBitmap 使用期间不可修改或释放。</para>
        /// <para>返回值与 <paramref name="image"/> 是不同对象，需分别管理生命周期。</para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="image"/> 为 null。</exception>
        public static SKBitmap ToSKBitmap(this SKImage image)
        {
            return SKBitmap.FromImage(image);
        }

        /// <summary>
        /// 修复 iPhone 照片的 EXIF 方向。
        /// </summary>
        /// <param name="image">源图像。</param>
        /// <returns>修正方向后的 SKImage。调用方负责释放，否则会造成内存泄漏。</returns>
        /// <remarks>
        /// <para>当方向为 TopLeft（无需旋转）时，返回 <paramref name="image"/> 的同一引用，请勿对返回值和 <paramref name="image"/> 分别释放，否则会重复释放同一对象。</para>
        /// <para>其他情况下返回新对象，与 <paramref name="image"/> 无关，可独立释放。</para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="image"/> 为 null。</exception>
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
            // 必须返回拷贝：bitmap/rotatedBitmap 即将 dispose，FromBitmap 会引用其像素
            return rotatedBitmap.ToSKImageCopy();
        }

        /// <summary>
        /// 获取图像的 EXIF 方向信息。
        /// </summary>
        /// <param name="image">源图像。</param>
        /// <returns>编码时的方向，无法读取时返回 <see cref="SKEncodedOrigin.TopLeft"/>。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="image"/> 为 null。</exception>
        public static SKEncodedOrigin GetOrientation(this SKImage image)
        {
            using var data = image.Encode();
            using var codec = SKCodec.Create(data);
            return codec?.EncodedOrigin ?? SKEncodedOrigin.TopLeft;
        }

        /// <summary>
        /// 检测图像的原始编码格式。
        /// </summary>
        /// <param name="image">源图像。</param>
        /// <returns>编码格式，无法检测时返回 null。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="image"/> 为 null。</exception>
        public static SKEncodedImageFormat? DetectImageFormat(SKImage image)
        {
            using var data = image.Encode();
            using var codec = SKCodec.Create(data);
            return codec?.EncodedFormat;
        }
    }
}
