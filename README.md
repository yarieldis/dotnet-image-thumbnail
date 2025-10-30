# .NET Image Thumbnail Library

A high-performance .NET 8 library for creating image thumbnails with support for multiple image formats and advanced processing capabilities. The library provides **two implementation options**:

- **SkiaSharp Implementation** (main branch): Cross-platform solution with superior image quality
- **GDI+ Implementation** (windows branch): Windows-native solution using System.Drawing

## 🚀 Features

- **Multiple Implementation Options**: Choose between SkiaSharp (cross-platform) or GDI+ (Windows-native)
- **High-Quality Thumbnail Generation**: Create thumbnails with variable width/height while maintaining aspect ratio
- **Advanced Image Processing**: Enhanced implementation with cropping, filtering, and format conversion
- **Multiple Image Formats**: Support for PNG, JPEG, WebP, AVIF, BMP, GIF, and ICO formats
- **Intelligent File Management**: Automatic thumbnail naming and cache management
- **Performance Optimized**: Built on industry-standard graphics libraries
- **Flexible Deployment**: Cross-platform with SkiaSharp or Windows-optimized with GDI+

## 📦 Installation

### Prerequisites

- .NET 8.0 or higher
- SkiaSharp 3.119.1 (for SkiaSharp implementation)
- System.Drawing (for GDI+ implementation on Windows)

### Package Installation

**For SkiaSharp Implementation (main branch):**
```bash
dotnet add package SkiaSharp
```

**For GDI+ Implementation (windows branch):**
```bash
# Switch to windows branch
git checkout windows
# No additional packages required - uses built-in System.Drawing
```

## 🏗️ Architecture

The library is built around a modular architecture with clear separation of concerns:

### Core Interfaces

- **`IImageHelper`**: Basic thumbnail creation and image saving functionality
- **`IImageDecoder`**: Image format detection and conversion utilities
- **`IImageQuantizer`**: Color quantization for optimized file sizes

- **`IImageThumbnailManager`**: High-level thumbnail management with caching

- **`IEnhancedImageHelper`**: Advanced features including cropping, filtering, and format conversion

### Choosing Your Implementation

**SkiaSharp Implementation (main branch)** - Recommended for:
- Cross-platform applications (Windows, macOS, Linux)
- Applications requiring advanced image processing features
- Modern image format support (WebP, AVIF)
- High-quality image rendering with anti-aliasing

**GDI+ Implementation (windows branch)** - Recommended for:
- Windows-only applications
- Minimal dependencies and smaller deployment size
- Integration with existing System.Drawing code
- Legacy system compatibility

## 📊 Implementation Comparison

| Feature | SkiaSharp (main) | GDI+ (windows) |
|---------|------------------|----------------|
| **Platform Support** | Windows, macOS, Linux | Windows only |
| **Dependencies** | SkiaSharp NuGet package | Built-in System.Drawing |
| **Modern Formats** | WebP, AVIF, all formats | Limited format support |
| **Image Quality** | Superior anti-aliasing | Standard quality |
| **Performance** | Optimized for all platforms | Windows-optimized |
| **Memory Usage** | Efficient with proper disposal | Native .NET memory management |
| **Advanced Features** | Filtering, cropping, effects | Basic operations |
| **Deployment Size** | Larger (includes SkiaSharp) | Smaller (no external deps) |

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

### For SkiaSharp Implementation (main branch)

- **SkiaSharp**: 3.119.1 - Cross-platform 2D graphics library
- **.NET**: 8.0 - Target framework

### For GDI+ Implementation (windows branch)

- **System.Drawing**: Built-in - Windows native graphics library
- **.NET**: 8.0 - Target framework
- **Windows**: Required platform
