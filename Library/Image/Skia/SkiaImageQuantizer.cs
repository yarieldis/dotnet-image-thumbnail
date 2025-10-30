using SkiaSharp;

namespace dotnet_image_thumbnail.Library.Image.Skia;

public class SkiaImageQuantizer : IImageQuantizer
{
    public byte[] Quantize(byte[] image, IImageDecoder.EncodedImageFormat? format)
    {
        using var inputStream = new MemoryStream(image);
        using var skBitmap = SKBitmap.Decode(inputStream) ?? throw new ArgumentException("Unable to decode image", nameof(image));

        // For now, perform a simple quantization by reducing color depth
        // You can enhance this later with more sophisticated algorithms
        var quantizedBitmap = PerformBasicQuantization(skBitmap);

        using var outputStream = new MemoryStream();
        var skImageFormat = SkiaImageDecoder.ConvertToSkiaImageFormat(format);

        using var image2 = SKImage.FromBitmap(quantizedBitmap);
        using var data = image2.Encode(skImageFormat, 90);

        quantizedBitmap.Dispose();
        return data.ToArray();
    }

    private static SKBitmap PerformBasicQuantization(SKBitmap originalBitmap)
    {
        // Create a new bitmap with reduced color precision
        var info = new SKImageInfo(originalBitmap.Width, originalBitmap.Height, SKColorType.Rgb565);
        var quantizedBitmap = new SKBitmap(info);

        // Copy pixels with color reduction
        var canvas = new SKCanvas(quantizedBitmap);
        canvas.DrawBitmap(originalBitmap, 0, 0);
        canvas.Dispose();

        return quantizedBitmap;
    }
}