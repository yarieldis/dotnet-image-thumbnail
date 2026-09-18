using dotnet_image_thumbnail.Library.Image;
using dotnet_image_thumbnail.Library.Image.Skia;
using SkiaSharp;

namespace dotnet_image_thumbnail.Tests;

public class SkiaImageHelperTests
{
    private readonly SkiaImageHelper _helper = new();

    [Fact]
    public void CreateThumbnailWithVariableWidth_ResizesAndPreservesAspectRatio()
    {
        string source = TestImages.CreateTempPngFile(200, 100);
        try
        {
            byte[] thumb = _helper.CreateThumbnailWithVariableWidth(source, IImageDecoder.EncodedImageFormat.Png, 80);

            using var decoded = SKBitmap.Decode(thumb) ?? throw new InvalidOperationException("Thumbnail did not decode.");
            Assert.Equal(80, decoded.Width);
            Assert.Equal(40, decoded.Height);
        }
        finally
        {
            File.Delete(source);
        }
    }

    [Fact]
    public void CreateThumbnailWithVariableHeight_ResizesAndPreservesAspectRatio()
    {
        string source = TestImages.CreateTempPngFile(200, 100);
        try
        {
            byte[] thumb = _helper.CreateThumbnailWithVariableHeight(source, IImageDecoder.EncodedImageFormat.Png, 50);

            using var decoded = SKBitmap.Decode(thumb) ?? throw new InvalidOperationException("Thumbnail did not decode.");
            Assert.Equal(100, decoded.Width);
            Assert.Equal(50, decoded.Height);
        }
        finally
        {
            File.Delete(source);
        }
    }

    [Fact]
    public void Save_WritesDecodableFile()
    {
        byte[] png = TestImages.CreatePng(32, 32);
        string destination = Path.Combine(Path.GetTempPath(), $"dit-save-{Guid.NewGuid():N}.png");

        try
        {
            bool saved = _helper.Save(png, IImageDecoder.EncodedImageFormat.Png, destination);

            Assert.True(saved);
            Assert.True(File.Exists(destination));

            using var decoded = SKBitmap.Decode(destination) ?? throw new InvalidOperationException("Saved image did not decode.");
            Assert.Equal(32, decoded.Width);
        }
        finally
        {
            File.Delete(destination);
        }
    }
}
