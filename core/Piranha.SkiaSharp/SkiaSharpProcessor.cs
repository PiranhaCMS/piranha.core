using SkiaSharp;

namespace Piranha.SkiaSharp;

/// <summary>
///  An alternative to ImageSharp as it now require a license
/// </summary>
public class SkiaSharpProcessor : IImageProcessor
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="onSize"></param>
    /// <exception cref="NullReferenceException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public void GetSize(Stream stream, Action<int, int> onSize)
    {
        var clonedStream = CloneStream(stream);
        using var codec = SKCodec.Create(clonedStream);
        onSize(codec.Info.Width, codec.Info.Height);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="bytes"></param>
    /// <param name="onSize"></param>
    public void GetSize(byte[] bytes, Action<int, int> onSize)
    {
        using var data = SKData.CreateCopy(bytes);
        using var codec = SKCodec.Create(data);
        onSize(codec.Info.Width, codec.Info.Height);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <param name="width"></param>
    public void Scale(Stream source, Stream dest, int width)
    {
        using var bitmap = SKBitmap.Decode(source);
        var ratio = (float)width / bitmap.Width;
        var height = (int)(bitmap.Height * ratio);
        using var resized = bitmap.Resize(
            new SKImageInfo(width, height),
            SKSamplingOptions.Default);
        SaveBitmap(resized, dest);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    public void Crop(Stream source, Stream dest, int width, int height)
    {
        using var bitmap = SKBitmap.Decode(source);
        var x = Math.Max(0, (bitmap.Width - width) / 2);
        var y = Math.Max(0, (bitmap.Height - height) / 2);
        using var cropped = new SKBitmap();
        if (bitmap.ExtractSubset(cropped, SKRectI.Create(x, y, width, height)))
        {
            SaveBitmap(cropped, dest);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    public void CropScale(Stream source, Stream dest, int width, int height)
    {
        var clonedSource = CloneStream(source);
        using var bitmap = SKBitmap.Decode(clonedSource);
        var scale = Math.Max(
            (float)width / bitmap.Width,
            (float)height / bitmap.Height);

        var scaledWidth = (int)(bitmap.Width * scale);
        var scaledHeight = (int)(bitmap.Height * scale);

        using var scaled = bitmap.Resize(
            new SKImageInfo(scaledWidth, scaledHeight),
            SKSamplingOptions.Default);

        var cropX = (scaledWidth - width) / 2;
        var cropY = (scaledHeight - height) / 2;

        using var result = new SKBitmap(width, height);
        using var canvas = new SKCanvas(result);        
        var srcRect = SKRect.Create(cropX, cropY, width, height);
        var destRect = SKRect.Create(0, 0, width, height);
        // Pass SKSamplingOptions.Default to fix CS0618
        canvas.DrawBitmap(scaled, srcRect, destRect, SKSamplingOptions.Default);

        SaveBitmap(result, dest);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    public void AutoOrient(Stream source, Stream dest)
    {
        var clonedSource1 = CloneStream(source);
        using var codec = SKCodec.Create(clonedSource1);        
        var clonedSource2 = CloneStream(clonedSource1);
        using var bitmap = SKBitmap.Decode(clonedSource2);        
        var oriented = ApplyOrientation(bitmap, codec.EncodedOrigin);
        SaveBitmap(oriented, dest);        
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="bitmap"></param>
    /// <param name="origin"></param>
    /// <returns></returns>
    private static SKBitmap ApplyOrientation(
        SKBitmap bitmap,
        SKEncodedOrigin origin)
    {
        if (origin == SKEncodedOrigin.TopLeft)
        {
            return bitmap.Copy();
        }

        using var surface = SKSurface.Create(
            new SKImageInfo(bitmap.Width, bitmap.Height));

        var canvas = surface.Canvas;

        switch (origin)
        {
            case SKEncodedOrigin.RightTop:
                canvas.RotateDegrees(90);
                canvas.Translate(0, -bitmap.Height);
                break;

            case SKEncodedOrigin.BottomRight:
                canvas.RotateDegrees(180);
                canvas.Translate(-bitmap.Width, -bitmap.Height);
                break;

            case SKEncodedOrigin.LeftBottom:
                canvas.RotateDegrees(270);
                canvas.Translate(-bitmap.Width, 0);
                break;
            default: return bitmap;
        }

        // Pass SKSamplingOptions.Default to satisfy the non-obsolete DrawBitmap overload
        canvas.DrawBitmap(bitmap, 0, 0, SKSamplingOptions.Default);

        return SKBitmap.FromImage(surface.Snapshot());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="bitmap"></param>
    /// <param name="dest"></param>
    private static void SaveBitmap(SKBitmap bitmap, Stream dest)
    {
        using var image = SKImage.FromBitmap(bitmap);
        using var data = image.Encode(SKEncodedImageFormat.Jpeg, 90);

        data.SaveTo(dest);
        dest.Position = 0;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static Stream CloneStream(Stream source)
    {
        if (source.CanSeek)
        {
            source.Position = 0;
        }
            
        var clone = new MemoryStream();
        source.CopyTo(clone);

        if (source.CanSeek)
        {
            source.Position = 0;
        }

        clone.Position = 0;
        return clone;
    }
}