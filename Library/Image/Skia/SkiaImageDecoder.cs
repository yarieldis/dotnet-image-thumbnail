using SkiaSharp;

namespace dotnet_image_thumbnail.Library.Image.Skia;

public class SkiaImageDecoder : IImageDecoder
{
    private readonly List<IImageDecoder.EncodedImageFormat> _supportedFormats;

    public SkiaImageDecoder()
    {
        _supportedFormats =
        [
            IImageDecoder.EncodedImageFormat.Png,
            IImageDecoder.EncodedImageFormat.Jpeg,
            IImageDecoder.EncodedImageFormat.Jpg,
            IImageDecoder.EncodedImageFormat.Webp,
            IImageDecoder.EncodedImageFormat.Bmp,
            IImageDecoder.EncodedImageFormat.Gif,
            IImageDecoder.EncodedImageFormat.Ico,
            IImageDecoder.EncodedImageFormat.Avif
        ];
    }

    public IImageDecoder.EncodedImageFormat? GetEncodedImageFormat(string filename)
    {
        var extension = Path.GetExtension(filename).TrimStart('.');

        if (Enum.TryParse<IImageDecoder.EncodedImageFormat>(extension, true, out var format))
        {
            return _supportedFormats.Contains(format) ? format : null;
        }

        return null;
    }

    public static SKEncodedImageFormat ConvertToSkiaImageFormat(IImageDecoder.EncodedImageFormat? format)
    {
        return format switch
        {
            IImageDecoder.EncodedImageFormat.Bmp => SKEncodedImageFormat.Bmp,
            IImageDecoder.EncodedImageFormat.Gif => SKEncodedImageFormat.Gif,
            IImageDecoder.EncodedImageFormat.Ico => SKEncodedImageFormat.Ico,
            IImageDecoder.EncodedImageFormat.Jpg => SKEncodedImageFormat.Jpeg,
            IImageDecoder.EncodedImageFormat.Jpeg => SKEncodedImageFormat.Jpeg,
            IImageDecoder.EncodedImageFormat.Png => SKEncodedImageFormat.Png,
            IImageDecoder.EncodedImageFormat.Webp => SKEncodedImageFormat.Webp,
            IImageDecoder.EncodedImageFormat.Avif => SKEncodedImageFormat.Avif,
            null => SKEncodedImageFormat.Png,
            _ => throw new ArgumentException($"Unsupported image format: {format}", nameof(format)),
        };
    }

    public static IImageDecoder.EncodedImageFormat? DetectImageFormat(byte[] imageData)
    {
        using var stream = new MemoryStream(imageData);
        using var codec = SKCodec.Create(stream);

        return codec?.EncodedFormat switch
        {
            SKEncodedImageFormat.Bmp => IImageDecoder.EncodedImageFormat.Bmp,
            SKEncodedImageFormat.Gif => IImageDecoder.EncodedImageFormat.Gif,
            SKEncodedImageFormat.Ico => IImageDecoder.EncodedImageFormat.Ico,
            SKEncodedImageFormat.Jpeg => IImageDecoder.EncodedImageFormat.Jpeg,
            SKEncodedImageFormat.Png => IImageDecoder.EncodedImageFormat.Png,
            SKEncodedImageFormat.Webp => IImageDecoder.EncodedImageFormat.Webp,
            SKEncodedImageFormat.Avif => IImageDecoder.EncodedImageFormat.Avif,
            _ => null
        };
    }
}