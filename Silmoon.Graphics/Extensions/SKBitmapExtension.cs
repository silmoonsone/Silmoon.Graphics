using SkiaSharp;
using System;

namespace Silmoon.Graphics.Extensions
{
    /// <summary>
    /// 提供 <see cref="SKBitmap"/> 的扩展方法，包括缩放、旋转、编码解码等操作。
    /// </summary>
    /// <remarks>
    /// 本类中返回 <see cref="SKBitmap"/> 或 <see cref="SKImage"/> 的方法，调用方负责释放返回值，否则会造成内存泄漏。
    /// 部分方法（如 <see cref="Resize"/> 在特定条件下）可能返回与输入相同的引用，此时请勿重复释放。
    /// </remarks>
    public static class SKBitmapExtension
    {
        /// <summary>
        /// 将位图缩放到指定尺寸。优先使用原生 Resize，失败时回退到 DrawImage。
        /// </summary>
        /// <param name="bitmap">源位图。</param>
        /// <param name="width">目标宽度。</param>
        /// <param name="height">目标高度。</param>
        /// <returns>缩放后的新位图。调用方负责释放，否则会造成内存泄漏。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="bitmap"/> 为 null。</exception>
        /// <exception cref="ArgumentException">源位图尺寸为 0，或 width/height 均为 0。</exception>
        /// <exception cref="ArgumentOutOfRangeException">width 或 height 小于 0。</exception>
        public static SKBitmap Resize(this SKBitmap bitmap, int width, int height) => Resize(bitmap, width, height, ifTargetSizeGreaterKeepOriginal: false, maintainAspectRatio: false);

        /// <summary>
        /// 将位图缩放到指定尺寸，支持“目标更大则保持原图”和“保持宽高比”选项。
        /// </summary>
        /// <param name="bitmap">源位图。</param>
        /// <param name="width">目标宽度，maintainAspectRatio 时可传 0 表示按比例计算。</param>
        /// <param name="height">目标高度，maintainAspectRatio 时可传 0 表示按比例计算。</param>
        /// <param name="ifTargetSizeGreaterKeepOriginal">为 true 且目标宽高均 ≥ 原图时，直接返回原图（同一实例）。</param>
        /// <param name="maintainAspectRatio">为 true 时按比例缩放；“装入”策略用 Floor 严格不超框。</param>
        /// <returns>缩放后的位图。调用方负责释放，否则会造成内存泄漏。</returns>
        /// <remarks>
        /// <para>当 <paramref name="ifTargetSizeGreaterKeepOriginal"/> 为 true 且目标宽高均 ≥ 原图时，返回的是 <paramref name="bitmap"/> 的同一引用（非 clone）。此时请勿对返回值和 <paramref name="bitmap"/> 分别释放，否则会重复释放同一对象。</para>
        /// <para>其他情况下返回新对象，与 <paramref name="bitmap"/> 无关，可独立释放。</para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="bitmap"/> 为 null。</exception>
        /// <exception cref="ArgumentException">源位图尺寸为 0，或解析后的 width/height 无效。</exception>
        /// <exception cref="ArgumentOutOfRangeException">width 或 height 小于 0。</exception>
        public static SKBitmap Resize(this SKBitmap bitmap, int width, int height, bool ifTargetSizeGreaterKeepOriginal, bool maintainAspectRatio)
        {
            if (bitmap == null) throw new ArgumentNullException(nameof(bitmap));
            if (bitmap.Width == 0 || bitmap.Height == 0) throw new ArgumentException("Source bitmap must have non-zero dimensions.", nameof(bitmap));
            if (width < 0) throw new ArgumentOutOfRangeException(nameof(width), width, "width must be >= 0.");
            if (height < 0) throw new ArgumentOutOfRangeException(nameof(height), height, "height must be >= 0.");
            if (width == 0 && height == 0) throw new ArgumentException("width and height cannot both be 0.");

            // 语义B：仅当目标宽高均 >= 原图时视为“放大”，跳过缩放（避免“变窄变高”被误挡）
            if (ifTargetSizeGreaterKeepOriginal && width >= bitmap.Width && height >= bitmap.Height)
                return bitmap; // 返回同一实例，非 clone

            // maintainAspectRatio：width 或 height 可传 0，表示按比例计算
            if (maintainAspectRatio)
            {
                float ar = (float)bitmap.Width / bitmap.Height;

                if (width > 0 && height == 0)
                    height = (int)MathF.Round(width / ar);
                else if (height > 0 && width == 0)
                    width = (int)MathF.Round(height * ar);
                else
                {
                    // 宽高均给定：按“装入”策略，Floor 保证不超出目标框
                    if ((float)width / height > ar) width = (int)MathF.Floor(height * ar);
                    else height = (int)MathF.Floor(width / ar);
                }
            }
            else
            {
                // 不保持比例：0 表示使用原图对应尺寸
                if (width == 0) width = bitmap.Width;
                if (height == 0) height = bitmap.Height;
            }

            if (width <= 0 || height <= 0) throw new ArgumentException("Resolved width/height must be > 0.");

            // 采样：缩小用 mipmap 提升质量，放大时 mipmap 无收益且可能更糊/更慢
            var downscale = width < bitmap.Width || height < bitmap.Height;
            var sampling = downscale
                ? new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.Linear)
                : new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.None);

            // 快路径：原生 Resize，避免创建 Canvas/Paint
            var info = new SKImageInfo(width, height, bitmap.ColorType, bitmap.AlphaType);
            var resized = bitmap.Resize(info, sampling);
            if (resized != null) return resized;

            // 兜底：Resize 返回 null 时，使用 DrawImage
            var dst = new SKBitmap(info);
            try
            {
                using (var canvas = new SKCanvas(dst))
                using (var image = SKImage.FromBitmap(bitmap))
                {
                    canvas.Clear(SKColors.Transparent);
                    canvas.DrawImage(image, new SKRect(0, 0, width, height), sampling);
                    canvas.Flush();
                }
                return dst;
            }
            catch
            {
                dst.Dispose();
                throw;
            }
        }

        /// <summary>
        /// 按指定宽度缩放位图，可选保持宽高比及“目标更大则保持原图”。
        /// </summary>
        /// <param name="bitmap">源位图。</param>
        /// <param name="width">目标宽度。</param>
        /// <param name="ifTargetSizeGreaterKeepOriginal">为 true 且目标宽度大于原图时，使用原图宽度。</param>
        /// <param name="maintainAspectRatio">为 true 时按比例计算高度，保证至少为 1。</param>
        /// <returns>缩放后的新位图，与 <paramref name="bitmap"/> 非同一引用。调用方负责释放，否则会造成内存泄漏。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="bitmap"/> 为 null。</exception>
        /// <exception cref="ArgumentException">源位图尺寸为 0，或 width 为 0。</exception>
        /// <exception cref="ArgumentOutOfRangeException">width 小于 0。</exception>
        public static SKBitmap ResizeWidth(this SKBitmap bitmap, int width, bool ifTargetSizeGreaterKeepOriginal, bool maintainAspectRatio = false)
        {
            if (bitmap == null) throw new ArgumentNullException(nameof(bitmap));
            if (bitmap.Width == 0 || bitmap.Height == 0) throw new ArgumentException("Source bitmap must have non-zero dimensions.", nameof(bitmap));
            if (width < 0) throw new ArgumentOutOfRangeException(nameof(width));
            if (width == 0) throw new ArgumentException("width cannot be 0.", nameof(width));

            if (ifTargetSizeGreaterKeepOriginal && bitmap.Width < width) width = bitmap.Width;

            int height = bitmap.Height;
            if (maintainAspectRatio)
            {
                double scale = (double)width / bitmap.Width;
                height = Math.Max(1, (int)Math.Round(bitmap.Height * scale));
            }

            return bitmap.Resize(width, height);
        }

        /// <summary>
        /// 按指定高度缩放位图，可选保持宽高比及“目标更大则保持原图”。
        /// </summary>
        /// <param name="bitmap">源位图。</param>
        /// <param name="height">目标高度。</param>
        /// <param name="ifTargetSizeGreaterKeepOriginal">为 true 且目标高度大于原图时，使用原图高度。</param>
        /// <param name="maintainAspectRatio">为 true 时按比例计算宽度，保证至少为 1。</param>
        /// <returns>缩放后的新位图，与 <paramref name="bitmap"/> 非同一引用。调用方负责释放，否则会造成内存泄漏。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="bitmap"/> 为 null。</exception>
        /// <exception cref="ArgumentException">源位图尺寸为 0，或 height 为 0。</exception>
        /// <exception cref="ArgumentOutOfRangeException">height 小于 0。</exception>
        public static SKBitmap ResizeHeight(this SKBitmap bitmap, int height, bool ifTargetSizeGreaterKeepOriginal, bool maintainAspectRatio = false)
        {
            if (bitmap == null) throw new ArgumentNullException(nameof(bitmap));
            if (bitmap.Width == 0 || bitmap.Height == 0) throw new ArgumentException("Source bitmap must have non-zero dimensions.", nameof(bitmap));
            if (height < 0) throw new ArgumentOutOfRangeException(nameof(height));
            if (height == 0) throw new ArgumentException("height cannot be 0.", nameof(height));

            if (ifTargetSizeGreaterKeepOriginal && bitmap.Height < height) height = bitmap.Height;

            int width = bitmap.Width;
            if (maintainAspectRatio)
            {
                double scale = (double)height / bitmap.Height;
                width = Math.Max(1, (int)Math.Round(bitmap.Width * scale));
            }

            return bitmap.Resize(width, height);
        }

        /// <summary>
        /// 将位图编码为字节数组。
        /// </summary>
        /// <param name="bitmap">源位图。</param>
        /// <param name="format">编码格式，默认 PNG。</param>
        /// <param name="quality">质量 0-100，PNG 为无损格式可忽略此参数。</param>
        /// <returns>编码后的字节数组。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="bitmap"/> 为 null。</exception>
        /// <exception cref="ArgumentException">位图尺寸为 0。</exception>
        /// <exception cref="InvalidOperationException">编码失败。</exception>
        public static byte[] GetBytes(this SKBitmap bitmap, SKEncodedImageFormat format = SKEncodedImageFormat.Png, int quality = 100)
        {
            ArgumentNullException.ThrowIfNull(bitmap);
            if (bitmap.Width == 0 || bitmap.Height == 0) throw new ArgumentException("Bitmap must have non-zero dimensions.", nameof(bitmap));
            using var data = bitmap.Encode(format, quality);
            return data == null ? throw new InvalidOperationException("Failed to encode bitmap.") : data.ToArray();
        }

        /// <summary>
        /// 从字节数组解码创建 SKBitmap。
        /// </summary>
        /// <param name="imageData">图像数据（支持 PNG、JPEG、WebP 等格式）。</param>
        /// <returns>解码后的位图，解码失败时返回 null。非 null 时调用方负责释放，否则会造成内存泄漏。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="imageData"/> 为 null。</exception>
        public static SKBitmap FromBytes(byte[] imageData)
        {
            ArgumentNullException.ThrowIfNull(imageData);
            return SKBitmap.Decode(imageData);
        }

        /// <summary>
        /// 从字节数组解码创建 SKBitmap（扩展方法）。
        /// </summary>
        /// <param name="imageData">图像数据（支持 PNG、JPEG、WebP 等格式）。</param>
        /// <returns>解码后的位图，解码失败时返回 null。非 null 时调用方负责释放，否则会造成内存泄漏。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="imageData"/> 为 null。</exception>
        public static SKBitmap ToSKBitmap(this byte[] imageData)
        {
            ArgumentNullException.ThrowIfNull(imageData);
            return SKBitmap.Decode(imageData);
        }

        /// <summary>
        /// 将位图转换为 SKImage（引用模式）。返回的 SKImage 引用原 bitmap 像素。
        /// </summary>
        /// <param name="bitmap">源位图。</param>
        /// <returns>引用 bitmap 像素的 SKImage，与 <paramref name="bitmap"/> 非同一对象但共享像素。调用方负责释放返回值，否则会造成内存泄漏。</returns>
        /// <remarks>
        /// <para>返回的 SKImage 与 <paramref name="bitmap"/> 共享像素，在 SKImage 使用期间不可修改或释放 bitmap。</para>
        /// <para>返回值与 <paramref name="bitmap"/> 是不同对象，需分别管理生命周期；释放 SKImage 后仍可释放 bitmap。若需像素解耦，请使用 <see cref="ToSKImageCopy"/>。</para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="bitmap"/> 为 null。</exception>
        /// <exception cref="ArgumentException">位图尺寸为 0。</exception>
        public static SKImage ToSKImage(this SKBitmap bitmap)
        {
            ArgumentNullException.ThrowIfNull(bitmap);
            if (bitmap.Width == 0 || bitmap.Height == 0) throw new ArgumentException("Bitmap must have non-zero dimensions.", nameof(bitmap));
            return SKImage.FromBitmap(bitmap);
        }

        /// <summary>
        /// 将位图转换为 SKImage（拷贝模式）。拷贝像素数据，与原 bitmap 解耦。
        /// </summary>
        /// <param name="bitmap">源位图。</param>
        /// <returns>拥有独立像素拷贝的 SKImage，与 <paramref name="bitmap"/> 完全解耦。调用方负责释放，否则会造成内存泄漏。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="bitmap"/> 为 null。</exception>
        /// <exception cref="ArgumentException">位图尺寸为 0。</exception>
        /// <exception cref="InvalidOperationException">位图无可访问的像素数据（如 PeekPixels 返回 null）。</exception>
        public static SKImage ToSKImageCopy(this SKBitmap bitmap)
        {
            ArgumentNullException.ThrowIfNull(bitmap);
            if (bitmap.Width == 0 || bitmap.Height == 0) throw new ArgumentException("Bitmap must have non-zero dimensions.", nameof(bitmap));
            using var pixmap = bitmap.PeekPixels();
            if (pixmap == null) throw new InvalidOperationException("Bitmap has no accessible pixel data.");
            return SKImage.FromPixelCopy(pixmap);
        }

        /// <summary>
        /// 按指定角度旋转位图。45-135° 或 225-315° 时交换宽高，其余角度保持原尺寸。
        /// </summary>
        /// <param name="bitmap">源位图。</param>
        /// <param name="degrees">旋转角度（度），支持任意值，会归一化到 0-360。</param>
        /// <returns>旋转后的新位图，与 <paramref name="bitmap"/> 非同一引用。调用方负责释放，否则会造成内存泄漏。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="bitmap"/> 为 null。</exception>
        /// <exception cref="ArgumentException">位图尺寸为 0。</exception>
        public static SKBitmap Rotate(this SKBitmap bitmap, float degrees)
        {
            ArgumentNullException.ThrowIfNull(bitmap);
            if (bitmap.Width == 0 || bitmap.Height == 0) throw new ArgumentException("Bitmap must have non-zero dimensions.", nameof(bitmap));

            // 归一化角度到 0-360（不四舍五入，保留精度），45-135° 或 225-315° 时宽高互换
            var normalized = (degrees % 360 + 360) % 360;
            var swapDimensions = (normalized >= 45 && normalized < 135) || (normalized >= 225 && normalized < 315);
            var (w, h) = swapDimensions ? (bitmap.Height, bitmap.Width) : (bitmap.Width, bitmap.Height);

            var rotatedBitmap = new SKBitmap(w, h);
            try
            {
                using var canvas = new SKCanvas(rotatedBitmap);
                canvas.Translate(w / 2f, h / 2f);
                canvas.RotateDegrees(degrees);
                canvas.DrawBitmap(bitmap, -bitmap.Width / 2f, -bitmap.Height / 2f);
                return rotatedBitmap;
            }
            catch
            {
                rotatedBitmap.Dispose();
                throw;
            }
        }
    }
}
