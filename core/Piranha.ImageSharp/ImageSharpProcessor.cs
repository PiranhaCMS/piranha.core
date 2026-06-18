/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using SkiaSharp;

namespace Piranha.ImageSharp;

public class ImageSharpProcessor : IImageProcessor
{
    private static readonly SKSamplingOptions Sampling = new(SKFilterMode.Linear, SKMipmapMode.Linear);

    public void GetSize(Stream stream, out int width, out int height)
    {
        using var data = SKData.Create(stream);
        using var codec = SKCodec.Create(data);
        width = codec.Info.Width;
        height = codec.Info.Height;
        stream.Position = 0;
    }

    public void GetSize(byte[] bytes, out int width, out int height)
    {
        using var data = SKData.Create(new MemoryStream(bytes));
        using var codec = SKCodec.Create(data);
        width = codec.Info.Width;
        height = codec.Info.Height;
    }

    public void Crop(Stream source, Stream dest, int width, int height)
    {
        var (bitmap, format) = Load(source);
        using (bitmap)
        {
            int x = width < bitmap.Width ? (bitmap.Width - width) / 2 : 0;
            int y = height < bitmap.Height ? (bitmap.Height - height) / 2 : 0;
            using var cropped = CropBitmap(bitmap, x, y, width, height);
            Save(cropped, dest, format);
        }
    }

    public void Scale(Stream source, Stream dest, int width)
    {
        var (bitmap, format) = Load(source);
        using (bitmap)
        {
            int height = (int)Math.Round(width * ((float)bitmap.Height / bitmap.Width));
            using var scaled = bitmap.Resize(new SKImageInfo(width, height), Sampling);
            Save(scaled, dest, format);
        }
    }

    public void CropScale(Stream source, Stream dest, int width, int height)
    {
        var (bitmap, format) = Load(source);
        using (bitmap)
        {
            var oldRatio = (float)bitmap.Height / bitmap.Width;
            var newRatio = (float)height / width;
            var cropWidth = bitmap.Width;
            var cropHeight = bitmap.Height;

            if (newRatio < oldRatio)
                cropHeight = (int)Math.Round(bitmap.Width * newRatio);
            else
                cropWidth = (int)Math.Round(bitmap.Height / newRatio);

            int x = cropWidth < bitmap.Width ? (bitmap.Width - cropWidth) / 2 : 0;
            int y = cropHeight < bitmap.Height ? (bitmap.Height - cropHeight) / 2 : 0;

            using var cropped = CropBitmap(bitmap, x, y, cropWidth, cropHeight);
            using var scaled = cropped.Resize(new SKImageInfo(width, height), Sampling);
            Save(scaled, dest, format);
        }
    }

    public void AutoOrient(Stream source, Stream dest)
    {
        using var data = SKData.Create(source);
        using var codec = SKCodec.Create(data);
        var origin = codec?.EncodedOrigin ?? SKEncodedOrigin.TopLeft;
        var format = codec?.EncodedFormat ?? SKEncodedImageFormat.Jpeg;
        using var bitmap = SKBitmap.Decode(data);
        using var oriented = ApplyOrientation(bitmap, origin);
        Save(oriented, dest, format);
        dest.Position = 0;
    }

    private static (SKBitmap bitmap, SKEncodedImageFormat format) Load(Stream source)
    {
        using var data = SKData.Create(source);
        using var codec = SKCodec.Create(data);
        var format = codec?.EncodedFormat ?? SKEncodedImageFormat.Jpeg;
        return (SKBitmap.Decode(data), format);
    }

    private static SKBitmap CropBitmap(SKBitmap src, int x, int y, int width, int height)
    {
        var dst = new SKBitmap(width, height);
        using var canvas = new SKCanvas(dst);
        canvas.DrawBitmap(src,
            new SKRect(x, y, x + width, y + height),
            new SKRect(0, 0, width, height));
        return dst;
    }

    private static void Save(SKBitmap bitmap, Stream dest, SKEncodedImageFormat format, int quality = 90)
    {
        using var encoded = bitmap.Encode(format, quality);
        encoded.SaveTo(dest);
    }

    private static SKBitmap ApplyOrientation(SKBitmap src, SKEncodedOrigin origin)
    {
        if (origin == SKEncodedOrigin.TopLeft)
            return src.Copy();

        int w = src.Width, h = src.Height;
        bool swapDims = origin is SKEncodedOrigin.LeftTop or SKEncodedOrigin.RightTop
                                 or SKEncodedOrigin.RightBottom or SKEncodedOrigin.LeftBottom;

        var dst = new SKBitmap(swapDims ? h : w, swapDims ? w : h);
        using var canvas = new SKCanvas(dst);

        // Each matrix maps source pixel (x,y) to the correct destination position
        SKMatrix matrix = origin switch
        {
            SKEncodedOrigin.TopRight    => new SKMatrix { ScaleX = -1, SkewX =  0, TransX = w, SkewY =  0, ScaleY =  1, TransY = 0, Persp2 = 1 }, // flip H
            SKEncodedOrigin.BottomRight => new SKMatrix { ScaleX = -1, SkewX =  0, TransX = w, SkewY =  0, ScaleY = -1, TransY = h, Persp2 = 1 }, // 180°
            SKEncodedOrigin.BottomLeft  => new SKMatrix { ScaleX =  1, SkewX =  0, TransX = 0, SkewY =  0, ScaleY = -1, TransY = h, Persp2 = 1 }, // flip V
            SKEncodedOrigin.LeftTop     => new SKMatrix { ScaleX =  0, SkewX =  1, TransX = 0, SkewY =  1, ScaleY =  0, TransY = 0, Persp2 = 1 }, // transpose
            SKEncodedOrigin.RightTop    => new SKMatrix { ScaleX =  0, SkewX = -1, TransX = h, SkewY =  1, ScaleY =  0, TransY = 0, Persp2 = 1 }, // 90° CW
            SKEncodedOrigin.RightBottom => new SKMatrix { ScaleX =  0, SkewX = -1, TransX = h, SkewY = -1, ScaleY =  0, TransY = w, Persp2 = 1 }, // transverse
            SKEncodedOrigin.LeftBottom  => new SKMatrix { ScaleX =  0, SkewX =  1, TransX = 0, SkewY = -1, ScaleY =  0, TransY = w, Persp2 = 1 }, // 90° CCW
            _                         => SKMatrix.CreateIdentity()
        };

        canvas.SetMatrix(matrix);
        canvas.DrawBitmap(src, 0, 0);
        canvas.Flush();
        return dst;
    }
}
