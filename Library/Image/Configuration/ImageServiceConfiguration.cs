using Microsoft.Extensions.DependencyInjection;
using dotnet_image_thumbnail.Library.Image.Skia;

namespace dotnet_image_thumbnail.Library.Image.Configuration;

public static class ImageServiceConfiguration
{
    public enum ImageProvider
    {
        Skia,
        AdvancedSkia
    }

    public static IServiceCollection AddImageServices(this IServiceCollection services, ImageProvider provider = ImageProvider.Skia)
    {
        return provider switch
        {
            ImageProvider.Skia => services.AddSkiaImageServices(),
            ImageProvider.AdvancedSkia => services.AddAdvancedSkiaImageServices(),
            _ => throw new ArgumentException($"Unknown image provider: {provider}")
        };
    }

    private static IServiceCollection AddSkiaImageServices(this IServiceCollection services)
    {
        services.AddScoped<IImageQuantizer, SkiaImageQuantizer>();
        services.AddScoped<IImageHelper, SkiaImageHelper>();
        services.AddScoped<IImageDecoder, SkiaImageDecoder>();
        return services;
    }

    private static IServiceCollection AddAdvancedSkiaImageServices(this IServiceCollection services)
    {
        services.AddScoped<IImageQuantizer, SkiaImageQuantizer>();
        services.AddScoped<IImageHelper, AdvancedSkiaImageHelper>();
        services.AddScoped<IImageDecoder, SkiaImageDecoder>();
        return services;
    }
}