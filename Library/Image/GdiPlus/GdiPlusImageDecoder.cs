using System.Drawing.Imaging;

namespace dotnet_image_thumbnail.Library.Image.GdiPlus;

public class GdiPlusImageDecoder : IImageDecoder
{
    private readonly List<IImageDecoder.EncodedImageFormat> _allImgCodecs = [];

    public GdiPlusImageDecoder()
    {
#pragma warning disable CA1416 // Validate platform compatibility
        ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();
#pragma warning restore CA1416 // Validate platform compatibility
        encoders ??= new ImageCodecInfo[1];
#pragma warning disable CA1416 // Validate platform compatibility
        ImageCodecInfo[] decoders = ImageCodecInfo.GetImageDecoders();
#pragma warning restore CA1416 // Validate platform compatibility
        decoders ??= new ImageCodecInfo[1];

#pragma warning disable CA1416 // Validate platform compatibility
        var codecs = encoders.Concat(decoders).ToArray();
#pragma warning restore CA1416 // Validate platform compatibility

        for (int i = 0; i <= codecs.Length - 1; i++)
        {
#pragma warning disable CA1416 // Validate platform compatibility
            string[]? extensions = codecs[i].FilenameExtension?.Split(';');
#pragma warning restore CA1416 // Validate platform compatibility
            for (int j = 0; j < extensions?.Length; j++)
            {
                try
                {
                    var extension = extensions[j].Replace("*.", "");
                    var format = (IImageDecoder.EncodedImageFormat)Enum.Parse(typeof(IImageDecoder.EncodedImageFormat), extension, true);
                    if (_allImgCodecs?.FirstOrDefault(f => f == format) != null)
                    {
                        _allImgCodecs.Add(format);
                    }
                }
                catch
                {

                }
            }
        }
    }

    public IImageDecoder.EncodedImageFormat? GetEncodedImageFormat(string filename)
    {
        var extension = filename.AsSpan(filename.LastIndexOf('.') + 1);
        var format = (IImageDecoder.EncodedImageFormat)Enum.Parse(typeof(IImageDecoder.EncodedImageFormat), extension, true);

        return _allImgCodecs?.FirstOrDefault(f => f == format);
    }

    public static ImageFormat ConvertImageFormat(IImageDecoder.EncodedImageFormat? format)
    {
#pragma warning disable CA1416 // Validate platform compatibility
        return format switch
        {
            IImageDecoder.EncodedImageFormat.Bmp => ImageFormat.Bmp,
            IImageDecoder.EncodedImageFormat.Gif => ImageFormat.Gif,
            IImageDecoder.EncodedImageFormat.Ico => ImageFormat.Icon,
            IImageDecoder.EncodedImageFormat.Jpg => ImageFormat.Jpeg,
            IImageDecoder.EncodedImageFormat.Jpeg => ImageFormat.Jpeg,
            IImageDecoder.EncodedImageFormat.Png => ImageFormat.Png,
            IImageDecoder.EncodedImageFormat.Webp => ImageFormat.Webp,
            _ => throw new ArgumentException("Unsupported image format", nameof(format)),
        };
#pragma warning restore CA1416 // Validate platform compatibility
    }
}
