using dotnet_image_thumbnail.Library.Image;
using dotnet_image_thumbnail.Library.Image.Skia;
using SkiaSharp;

namespace dotnet_image_thumbnail.Tests;

public class SkiaImageQuantizerTests
{
    private readonly SkiaImageQuantizer _quantizer = new();

    [Fact]
    public void Quantize_ReturnsDecodableImage()
    {
        byte[] png = TestImages.CreatePng(64, 64);
        byte[] quantized = _quantizer.Quantize(png, IImageDecoder.EncodedImageFormat.Png);

        Assert.NotEmpty(quantized);
        using var decoded = SKBitmap.Decode(quantized) ?? throw new InvalidOperationException("Quantized image did not decode.");
        Assert.Equal(64, decoded.Width);
    }
}
