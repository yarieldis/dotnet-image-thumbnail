using SkiaSharp;

namespace dotnet_image_thumbnail.Library.Image.Skia;

/// <summary>
/// Quantizes images using the SkiaSharp implementation to reduce color depth and file size.
/// </summary>
public class SkiaImageQuantizer : IImageQuantizer
{
    /// <summary>
    /// Quantizes an image to reduce the number of colors while preserving visual quality.
    /// </summary>
    /// <param name="image">The image data as a byte array to be quantized.</param>
    /// <param name="format">The format of the input image. If <c>null</c>, the format will be auto-detected.</param>
    /// <returns>A byte array containing the quantized image data with reduced color palette.</returns>
    public byte[] Quantize(byte[] image, IImageDecoder.EncodedImageFormat? format)
    {
        using var skBitmap = SKBitmap.Decode(image) ?? throw new ArgumentException("Unable to decode image", nameof(image));

        // For now, perform a simple quantization by reducing color depth
        // You can enhance this later with more sophisticated algorithms
        using var quantizedBitmap = PerformBasicQuantization(skBitmap);

        var skImageFormat = SkiaImageDecoder.ConvertToSkiaImageFormat(format);

        using var image2 = SKImage.FromBitmap(quantizedBitmap);
        using var data = image2.Encode(skImageFormat, 90);

        return data.ToArray();
    }

    private static SKBitmap PerformBasicQuantization(SKBitmap originalBitmap)
    {
        // Create a new bitmap with reduced color precision
        var info = new SKImageInfo(originalBitmap.Width, originalBitmap.Height, SKColorType.Rgb565);
        var quantizedBitmap = new SKBitmap(info);

        // Copy pixels with color reduction
        using var canvas = new SKCanvas(quantizedBitmap);
        canvas.DrawBitmap(originalBitmap, 0, 0);

        return quantizedBitmap;
    }
}