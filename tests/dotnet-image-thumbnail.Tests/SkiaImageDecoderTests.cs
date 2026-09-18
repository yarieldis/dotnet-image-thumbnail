using dotnet_image_thumbnail.Library.Image;
using dotnet_image_thumbnail.Library.Image.Skia;

namespace dotnet_image_thumbnail.Tests;

public class SkiaImageDecoderTests
{
    private readonly SkiaImageDecoder _decoder = new();

    [Theory]
    [InlineData("image.png", IImageDecoder.EncodedImageFormat.Png)]
    [InlineData("image.jpg", IImageDecoder.EncodedImageFormat.Jpg)]
    [InlineData("image.jpeg", IImageDecoder.EncodedImageFormat.Jpeg)]
    [InlineData("image.gif", IImageDecoder.EncodedImageFormat.Gif)]
    [InlineData("image.webp", IImageDecoder.EncodedImageFormat.Webp)]
    [InlineData("image.bmp", IImageDecoder.EncodedImageFormat.Bmp)]
    [InlineData("image.ico", IImageDecoder.EncodedImageFormat.Ico)]
    [InlineData("image.avif", IImageDecoder.EncodedImageFormat.Avif)]
    public void GetEncodedImageFormat_SupportedExtension_ReturnsFormat(string filename, IImageDecoder.EncodedImageFormat expected)
    {
        Assert.Equal(expected, _decoder.GetEncodedImageFormat(filename));
    }

    [Fact]
    public void GetEncodedImageFormat_UnsupportedExtension_ReturnsNull()
    {
        Assert.Null(_decoder.GetEncodedImageFormat("image.tiff"));
    }

    [Fact]
    public void DetectImageFormat_PngBytes_ReturnsPng()
    {
        byte[] png = TestImages.CreatePng(8, 8);
        Assert.Equal(IImageDecoder.EncodedImageFormat.Png, SkiaImageDecoder.DetectImageFormat(png));
    }
}
