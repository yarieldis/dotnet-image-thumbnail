using dotnet_image_thumbnail.Library.Image;
using dotnet_image_thumbnail.Library.Image.Configuration;
using dotnet_image_thumbnail.Library.Image.GdiPlus;
using Microsoft.Extensions.DependencyInjection;
using SkiaSharp;

namespace dotnet_image_thumbnail.Tests;

public class GdiPlusTests
{
    [Fact]
    public void GdiPlusProvider_RegistersAndCreatesThumbnail()
    {
        // GDI+ is Windows-only; pass trivially on other platforms.
        if (!OperatingSystem.IsWindows())
            return;

        var services = new ServiceCollection();
        services.AddImageServices(ImageServiceConfiguration.ImageProvider.GdiPlus);
        using var provider = services.BuildServiceProvider();

        Assert.IsType<GdiPlusImageHelper>(provider.GetRequiredService<IImageHelper>());
        Assert.IsType<GdiPlusImageDecoder>(provider.GetRequiredService<IImageDecoder>());
        Assert.IsType<GdiPlusImageQuantizer>(provider.GetRequiredService<IImageQuantizer>());

        string source = TestImages.CreateTempPngFile(100, 50);
        try
        {
            var helper = provider.GetRequiredService<IImageHelper>();
            byte[] thumb = helper.CreateThumbnailWithVariableWidth(source, IImageDecoder.EncodedImageFormat.Png, 50);

            Assert.NotEmpty(thumb);
            using var decoded = SKBitmap.Decode(thumb) ?? throw new InvalidOperationException("GDI+ thumbnail did not decode.");
            Assert.Equal(50, decoded.Width);
        }
        finally
        {
            File.Delete(source);
        }
    }
}
