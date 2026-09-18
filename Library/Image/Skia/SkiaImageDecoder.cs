using SkiaSharp;

namespace dotnet_image_thumbnail.Library.Image.Skia;

/// <summary>
/// Decodes image formats using the SkiaSharp implementation.
/// </summary>
public class SkiaImageDecoder : IImageDecoder
{
    private readonly List<IImageDecoder.EncodedImageFormat> _supportedFormats;

    /// <summary>
    /// Initializes a new instance of the <see cref="SkiaImageDecoder"/> class.
    /// </summary>
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

    /// <summary>
    /// Determines the encoded image format based on the file name.
    /// </summary>
    /// <param name="filename">The path or name of the image file to analyze.</param>
    /// <returns>The detected format if supported by SkiaSharp; otherwise, <c>null</c>.</returns>
    public IImageDecoder.EncodedImageFormat? GetEncodedImageFormat(string filename)
    {
        ReadOnlySpan<char> extension = Path.GetExtension(filename).AsSpan().TrimStart('.');

        if (Enum.TryParse<IImageDecoder.EncodedImageFormat>(extension, true, out var format))
        {
            return _supportedFormats.Contains(format) ? format : null;
        }

        return null;
    }

    /// <summary>
    /// Converts a library image format to the corresponding SkiaSharp <see cref="SKEncodedImageFormat"/>.
    /// </summary>
    /// <param name="format">The image format to convert. If <c>null</c>, PNG is used.</param>
    /// <returns>The matching SkiaSharp <see cref="SKEncodedImageFormat"/>.</returns>
    /// <exception cref="ArgumentException">Thrown when the format is not supported.</exception>
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

    /// <summary>
    /// Detects the image format from the byte content of an image.
    /// </summary>
    /// <param name="imageData">The image data as a byte array to analyze.</param>
    /// <returns>The detected format if recognized; otherwise, <c>null</c>.</returns>
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