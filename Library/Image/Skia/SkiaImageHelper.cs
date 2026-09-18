using SkiaSharp;

namespace dotnet_image_thumbnail.Library.Image.Skia;

/// <summary>
/// Creates thumbnails and saves images using the SkiaSharp implementation.
/// </summary>
public class SkiaImageHelper : IImageHelper
{
    /// <summary>
    /// Creates a thumbnail image with a variable height while maintaining aspect ratio.
    /// </summary>
    /// <param name="filename">The path to the source image file.</param>
    /// <param name="imageFormat">The desired output format for the thumbnail. If <c>null</c>, the original format is used.</param>
    /// <param name="height">The target height for the thumbnail in pixels. If <c>null</c>, the original height is used.</param>
    /// <returns>A byte array containing the thumbnail image data.</returns>
    public byte[] CreateThumbnailWithVariableHeight(string filename, IImageDecoder.EncodedImageFormat? imageFormat, int? height)
    {
        using var skBitmap = SKBitmap.Decode(filename) ?? throw new ArgumentException("Unable to decode image from file", nameof(filename));
        if (skBitmap.Height <= height)
        {
            return SaveBitmapToByteArray(skBitmap, imageFormat);
        }

        int iHeight = height ?? skBitmap.Height;
        decimal ratio = (decimal)skBitmap.Height / iHeight;
        int newHeight = (int)(skBitmap.Height / ratio);
        int newWidth = (int)(skBitmap.Width / ratio);

        return CreateThumbnail(skBitmap, imageFormat, newWidth, newHeight);
    }

    /// <summary>
    /// Creates a thumbnail image with a variable width while maintaining aspect ratio.
    /// </summary>
    /// <param name="filename">The path to the source image file.</param>
    /// <param name="imageFormat">The desired output format for the thumbnail. If <c>null</c>, the original format is used.</param>
    /// <param name="width">The target width for the thumbnail in pixels. If <c>null</c>, the original width is used.</param>
    /// <returns>A byte array containing the thumbnail image data.</returns>
    public byte[] CreateThumbnailWithVariableWidth(string filename, IImageDecoder.EncodedImageFormat? imageFormat, int? width)
    {
        using var skBitmap = SKBitmap.Decode(filename) ?? throw new ArgumentException("Unable to decode image from file", nameof(filename));
        if (skBitmap.Width <= width)
        {
            return SaveBitmapToByteArray(skBitmap, imageFormat);
        }

        int iWidth = width ?? skBitmap.Width;
        decimal ratio = (decimal)skBitmap.Width / iWidth;
        int newHeight = (int)(skBitmap.Height / ratio);
        int newWidth = (int)(skBitmap.Width / ratio);

        return CreateThumbnail(skBitmap, imageFormat, newWidth, newHeight);
    }

    /// <summary>
    /// Saves image data to a file with the specified format.
    /// </summary>
    /// <param name="content">The image data as a byte array to be saved.</param>
    /// <param name="imageFormat">The format in which to save the image.</param>
    /// <param name="filename">The path where the image file will be saved.</param>
    /// <returns><c>true</c> if the image was saved successfully; otherwise, <c>false</c>.</returns>
    public bool Save(byte[] content, IImageDecoder.EncodedImageFormat imageFormat, string filename)
    {
        try
        {
            using var skBitmap = SKBitmap.Decode(content);

            if (skBitmap == null)
                return false;

            var skImageFormat = SkiaImageDecoder.ConvertToSkiaImageFormat(imageFormat);

            using var image = SKImage.FromBitmap(skBitmap);
            using var data = image.Encode(skImageFormat, 90);

            File.WriteAllBytes(filename, data.AsSpan());
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static byte[] CreateThumbnail(SKBitmap originalBitmap, IImageDecoder.EncodedImageFormat? imageFormat, int width, int height)
    {
        try
        {
            using var resizedBitmap = originalBitmap.Resize(new SKImageInfo(width, height), SKSamplingOptions.Default);
            if (resizedBitmap == null)
                return [];

            return SaveBitmapToByteArray(resizedBitmap, imageFormat);
        }
        catch
        {
            return [];
        }
    }

    private static byte[] SaveBitmapToByteArray(SKBitmap bitmap, IImageDecoder.EncodedImageFormat? imageFormat)
    {
        using var image = SKImage.FromBitmap(bitmap);
        var skImageFormat = SkiaImageDecoder.ConvertToSkiaImageFormat(imageFormat);

        using var data = image.Encode(skImageFormat, 90);
        return data.ToArray();
    }
}