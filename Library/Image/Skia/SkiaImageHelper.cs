using SkiaSharp;

namespace dotnet_image_thumbnail.Library.Image.Skia;

public class SkiaImageHelper : IImageHelper
{
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

    public bool Save(byte[] content, IImageDecoder.EncodedImageFormat imageFormat, string filename)
    {
        try
        {
            using var inputStream = new MemoryStream(content);
            using var skBitmap = SKBitmap.Decode(inputStream);

            if (skBitmap == null)
                return false;

            var skImageFormat = SkiaImageDecoder.ConvertToSkiaImageFormat(imageFormat);

            using var image = SKImage.FromBitmap(skBitmap);
            using var data = image.Encode(skImageFormat, 90);

            File.WriteAllBytes(filename, data.ToArray());
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