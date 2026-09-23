using dotnet_image_thumbnail.Library.Image;
using dotnet_image_thumbnail.Library.Image.Configuration;
using dotnet_image_thumbnail.Library.Image.Skia;
using Microsoft.Extensions.DependencyInjection;

namespace dotnet_image_thumbnail.Tests;

public class ImageServiceConfigurationTests
{
    [Fact]
    public void AddImageServices_Skia_RegistersSkiaImplementations()
    {
        var services = new ServiceCollection();
        services.AddImageServices(ImageServiceConfiguration.ImageProvider.Skia);
        using var provider = services.BuildServiceProvider();

        Assert.IsType<SkiaImageHelper>(provider.GetRequiredService<IImageHelper>());
        Assert.IsType<SkiaImageDecoder>(provider.GetRequiredService<IImageDecoder>());
        Assert.IsType<SkiaImageQuantizer>(provider.GetRequiredService<IImageQuantizer>());
        Assert.IsType<ImageThumbnailManager>(provider.GetRequiredService<IImageThumbnailManager>());
    }

    [Fact]
    public void AddImageServices_DefaultsToSkia()
    {
        var services = new ServiceCollection();
        services.AddImageServices();
        using var provider = services.BuildServiceProvider();

        Assert.IsType<SkiaImageHelper>(provider.GetRequiredService<IImageHelper>());
    }

    [Fact]
    public void AddImageServices_AdvancedSkia_RegistersAdvancedHelper()
    {
        var services = new ServiceCollection();
        services.AddImageServices(ImageServiceConfiguration.ImageProvider.AdvancedSkia);
        using var provider = services.BuildServiceProvider();

        Assert.IsType<AdvancedSkiaImageHelper>(provider.GetRequiredService<IImageHelper>());
        Assert.IsType<SkiaImageDecoder>(provider.GetRequiredService<IImageDecoder>());
    }

    [Fact]
    public void AddImageServices_UnknownProvider_Throws()
    {
        var services = new ServiceCollection();
        Assert.Throws<ArgumentException>(() => services.AddImageServices((ImageServiceConfiguration.ImageProvider)999));
    }
}
