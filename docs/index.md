# .NET Image Thumbnail Library

[![NuGet](https://img.shields.io/nuget/v/DotnetImageThumbnail.svg?label=NuGet)](https://www.nuget.org/packages/DotnetImageThumbnail/)

A high-performance .NET 10 library for creating image thumbnails with support for multiple image formats and advanced processing capabilities. The library provides **three providers**, selected at runtime via dependency injection:

- **Skia** (`ImageProvider.Skia`): Cross-platform solution with superior image quality
- **Advanced Skia** (`ImageProvider.AdvancedSkia`): Cross-platform with advanced processing (cropping, filtering, format conversion)
- **GDI+** (`ImageProvider.GdiPlus`): Windows-native solution using System.Drawing

## 🚀 Features

- **Multiple Implementation Options**: Choose between SkiaSharp (cross-platform) or GDI+ (Windows-native) via dependency injection
- **High-Quality Thumbnail Generation**: Create thumbnails with variable width/height while maintaining aspect ratio
- **Advanced Image Processing**: Enhanced implementation with cropping, filtering, and format conversion
- **Multiple Image Formats**: Support for PNG, JPEG, WebP, AVIF, BMP, GIF, and ICO formats
- **Intelligent File Management**: Automatic thumbnail naming and cache management
- **Performance Optimized**: Built on industry-standard graphics libraries
- **Flexible Deployment**: Cross-platform with SkiaSharp or Windows-optimized with GDI+

## 📦 Installation

### Prerequisites

- .NET 10.0 or higher
- SkiaSharp 3.119.4 (SkiaSharp providers)
- System.Drawing.Common 10.0.12 (GDI+ provider, Windows-only)
- Codenet.Drawing.Common.GdiPlus 2.0.4 (GDI+ provider)

### Package Installation

Install from [NuGet](https://www.nuget.org/packages/DotnetImageThumbnail/):
```bash
dotnet add package DotnetImageThumbnail
```

Build the library:
```bash
dotnet build
```

Register image services and choose a provider:
```csharp
using dotnet_image_thumbnail.Library.Image.Configuration;

services.AddImageServices(ImageProvider.Skia);         // default, cross-platform
services.AddImageServices(ImageProvider.AdvancedSkia); // cross-platform, advanced features
services.AddImageServices(ImageProvider.GdiPlus);      // Windows-only
```

## 🖥️ Usage

The easiest way to use the library is through `IImageThumbnailManager`. It handles
thumbnail naming, generation, and caching for you — you only provide the source
image path and the dimensions you want.

```csharp
using dotnet_image_thumbnail.Library.Image;
using dotnet_image_thumbnail.Library.Image.Configuration;

// Register once (e.g. in Program.cs)
builder.Services.AddImageServices(); // defaults to ImageProvider.Skia

// Inject and use
public class MyController(IImageThumbnailManager thumbnails)
{
    public string GetThumbnail(string imagePath)
    {
        // Creates the thumbnail on disk if missing (or stale) and returns a
        // URL-safe file name. Pass width OR height to preserve aspect ratio.
        return thumbnails.RetrieveThumbnailFileName(
            imagePath,
            thumbnailWidth: 300,
            thumbnailHeight: null);
    }
}
```

- `thumbnailWidth` only → fixed width, variable height (aspect ratio preserved)
- `thumbnailHeight` only → fixed height, variable width
- Both `null` → throws, since no target dimension was provided

Thumbnails are cached on disk: a thumbnail is only regenerated when the original
file has been modified more recently than the thumbnail.

## 🏗️ Architecture

The library is built around a modular architecture with clear separation of concerns:

### Core Interfaces

- **`IImageHelper`**: Basic thumbnail creation and image saving functionality
- **`IImageDecoder`**: Image format detection and conversion utilities
- **`IImageQuantizer`**: Color quantization for optimized file sizes

- **`IImageThumbnailManager`**: High-level thumbnail management with caching — **the recommended entry point** (see [Usage](#usage))

- **`IEnhancedImageHelper`**: Advanced features including cropping, filtering, and format conversion

See [Extending the Library](extending-the-library.md) for a guide to implementing your own versions of these interfaces and wiring them into your solution.

### Choosing a Provider

**SkiaSharp** (`ImageProvider.Skia` / `ImageProvider.AdvancedSkia`) - Recommended for:
- Cross-platform applications (Windows, macOS, Linux)
- Applications requiring advanced image processing features
- Modern image format support (WebP, AVIF)
- High-quality image rendering with anti-aliasing

**GDI+** (`ImageProvider.GdiPlus`) - Recommended for:
- Windows-only applications
- Integration with existing System.Drawing code
- Legacy system compatibility

## 📊 Implementation Comparison

| Feature | SkiaSharp | GDI+ |
|---------|-----------|------|
| **Platform Support** | Windows, macOS, Linux | Windows only |
| **Dependencies** | SkiaSharp NuGet package | System.Drawing.Common + Codenet.Drawing.Common.GdiPlus |
| **Modern Formats** | WebP, AVIF, all formats | Limited format support |
| **Image Quality** | Superior anti-aliasing | Standard quality |
| **Performance** | Optimized for all platforms | Windows-optimized |
| **Memory Usage** | Efficient with proper disposal | Native .NET memory management |
| **Advanced Features** | Filtering, cropping, effects | Basic operations |
| **Deployment Size** | Larger (includes SkiaSharp native libs) | Windows-only |

## 🎨 Supported Image Formats

| Format | Extension | Read | Write | Compression |
|--------|-----------|------|-------|-------------|
| PNG    | .png      | ✅   | ✅    | Lossless    |
| JPEG   | .jpg, .jpeg | ✅ | ✅    | Lossy       |
| WebP   | .webp     | ✅   | ✅    | Both        |
| AVIF   | .avif     | ✅   | ✅    | Advanced    |
| BMP    | .bmp      | ✅   | ✅    | Uncompressed|
| GIF    | .gif      | ✅   | ✅    | Lossless    |
| ICO    | .ico      | ✅   | ✅    | Various     |

### Thumbnail Naming Convention
Thumbnails are automatically named using the pattern:
```
Thumbnail_{OriginalName}_{Width}x{Height}.{Extension}
```

Example: `Thumbnail_photo_300x200.jpeg`

## 🐛 Known Issues

- Some advanced AVIF features may require additional codec support on certain platforms
- ICO format writing may have limitations with larger dimensions

## 📚 Dependencies

- **SkiaSharp**: 3.119.4 - Cross-platform 2D graphics library
- **Microsoft.Extensions.DependencyInjection.Abstractions**: 10.0.12 - DI service registration
- **System.Drawing.Common**: 10.0.12 - Windows native graphics (GDI+ provider, Windows-only)
- **Codenet.Drawing.Common.GdiPlus**: 2.0.4 - GDI+ color quantization (GDI+ provider)
- **.NET**: 10.0 - Target framework
