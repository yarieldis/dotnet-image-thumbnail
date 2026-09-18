using dotnet_image_thumbnail.Library.Image;
using dotnet_image_thumbnail.Library.Image.Skia;
using SkiaSharp;

namespace dotnet_image_thumbnail.Tests;

public class AdvancedSkiaImageHelperTests
{
    private readonly AdvancedSkiaImageHelper _helper = new();

    [Fact]
    public void CreateThumbnailWithCrop_ReturnsExactDimensions()
    {
        string source = TestImages.CreateTempPngFile(200, 100);
        try
        {
            byte[] thumb = _helper.CreateThumbnailWithCrop(source, IImageDecoder.EncodedImageFormat.Png, 50, 50);

            using var decoded = SKBitmap.Decode(thumb) ?? throw new InvalidOperationException("Thumbnail did not decode.");
            Assert.Equal(50, decoded.Width);
            Assert.Equal(50, decoded.Height);
        }
        finally
        {
            File.Delete(source);
        }
    }

    [Fact]
    public void CreateThumbnailFromBytes_ReturnsResizedImage()
    {
        byte[] png = TestImages.CreatePng(100, 100);
        byte[] thumb = _helper.CreateThumbnailFromBytes(png, IImageDecoder.EncodedImageFormat.Png, 40, 40);

        using var decoded = SKBitmap.Decode(thumb) ?? throw new InvalidOperationException("Thumbnail did not decode.");
        Assert.Equal(40, decoded.Width);
        Assert.Equal(40, decoded.Height);
    }

    [Fact]
    public void ConvertImageFormat_PngToJpeg_ReturnsJpegBytes()
    {
        byte[] png = TestImages.CreatePng(20, 20);
        byte[] jpeg = _helper.ConvertImageFormat(png, IImageDecoder.EncodedImageFormat.Png, IImageDecoder.EncodedImageFormat.Jpeg);

        Assert.NotEmpty(jpeg);
        Assert.Equal(IImageDecoder.EncodedImageFormat.Jpeg, SkiaImageDecoder.DetectImageFormat(jpeg));
    }

    [Fact]
    public void ApplyImageFilters_ReturnsDecodableImage()
    {
        byte[] png = TestImages.CreatePng(20, 20);
        byte[] filtered = _helper.ApplyImageFilters(png, IImageDecoder.EncodedImageFormat.Png, brightness: 1.2f);

        Assert.NotEmpty(filtered);
        using var decoded = SKBitmap.Decode(filtered) ?? throw new InvalidOperationException("Filtered image did not decode.");
        Assert.Equal(20, decoded.Width);
    }
}
