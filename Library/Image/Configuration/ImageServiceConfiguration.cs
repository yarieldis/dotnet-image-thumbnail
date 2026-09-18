using Microsoft.Extensions.DependencyInjection;
using dotnet_image_thumbnail.Library.Image.GdiPlus;
using dotnet_image_thumbnail.Library.Image.Skia;

namespace dotnet_image_thumbnail.Library.Image.Configuration;

/// <summary>
/// Provides extension methods to register image processing services in a dependency injection container.
/// </summary>
public static class ImageServiceConfiguration
{
    /// <summary>
    /// Specifies the available image processing providers.
    /// </summary>
    public enum ImageProvider
    {
        /// <summary>Uses the cross-platform SkiaSharp provider.</summary>
        Skia,
        /// <summary>Uses the SkiaSharp provider with advanced image operations.</summary>
        AdvancedSkia,
        /// <summary>Uses the Windows-only GDI+ provider.</summary>
        GdiPlus
    }

    /// <summary>
    /// Registers the image services for the specified provider.
    /// </summary>
    /// <param name="services">The service collection to which the image services are added.</param>
    /// <param name="provider">The image provider to register. Defaults to <see cref="ImageProvider.Skia"/>.</param>
    /// <returns>The service collection so calls can be chained.</returns>
    /// <exception cref="ArgumentException">Thrown when an unknown provider is specified.</exception>
    public static IServiceCollection AddImageServices(this IServiceCollection services, ImageProvider provider = ImageProvider.Skia)
    {
        return provider switch
        {
            ImageProvider.Skia => services.AddSkiaImageServices(),
            ImageProvider.AdvancedSkia => services.AddAdvancedSkiaImageServices(),
            ImageProvider.GdiPlus => services.AddGdiPlusImageServices(),
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

    private static IServiceCollection AddGdiPlusImageServices(this IServiceCollection services)
    {
        services.AddScoped<IImageQuantizer, GdiPlusImageQuantizer>();
        services.AddScoped<IImageHelper, GdiPlusImageHelper>();
        services.AddScoped<IImageDecoder, GdiPlusImageDecoder>();
        return services;
    }
}