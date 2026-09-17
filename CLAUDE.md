# CLAUDE.md - dotnet-image-thumbnail

This file provides guidance to Claude Code when working with the **dotnet-image-thumbnail** repository.

## Repository Overview

**Purpose**: A .NET 10 class library for creating high-quality image thumbnails with support for multiple image formats and advanced processing capabilities, with cross-platform (SkiaSharp) and Windows-native (GDI+) providers.

**Status**: Active development

**Technology Stack**:
- .NET 10.0 (class library, `Microsoft.NET.Sdk`)
- C# 14 (`LangVersion` 14.0)
- SkiaSharp 3.119.4
- Microsoft.Extensions.DependencyInjection.Abstractions 10.0.12
- System.Drawing.Common 10.0.12 (Windows-only, GDI+ provider)
- Codenet.Drawing.Common.GdiPlus 2.0.4 (GDI+ quantization)

## Project Structure

```
dotnet-image-thumbnail/
├── dotnet-image-thumbnail.sln            # Solution file
├── dotnet-image-thumbnail.csproj         # Project file (library, net10.0)
├── Library/
│   └── Image/
│       ├── IImageHelper.cs               # Basic thumbnail creation interface
│       ├── IEnhancedImageHelper.cs        # Advanced image processing interface
│       ├── IImageDecoder.cs              # Image format detection interface
│       ├── IImageQuantizer.cs            # Color quantization interface
│       ├── IImageThumbnailManager.cs     # High-level thumbnail management interface
│       ├── ImageThumbnailManager.cs      # Thumbnail manager implementation
│       ├── Configuration/
│       │   └── ImageServiceConfiguration.cs  # DI service registration extensions
│       ├── Skia/
│       │   ├── SkiaImageHelper.cs        # SkiaSharp basic image helper
│       │   ├── AdvancedSkiaImageHelper.cs # SkiaSharp advanced image helper
│       │   ├── SkiaImageDecoder.cs       # SkiaSharp image decoder
│       │   └── SkiaImageQuantizer.cs     # SkiaSharp color quantizer
│       └── GdiPlus/
│           ├── GdiPlusImageHelper.cs     # GDI+ image helper (Windows-only)
│           ├── GdiPlusImageDecoder.cs    # GDI+ image decoder (Windows-only)
│           └── GdiPlusImageQuantizer.cs  # GDI+ color quantizer (Windows-only)
└── README.md
```

## Branches

- **main**: Cross-platform SkiaSharp implementation plus the Windows-only GDI+ provider (selectable at runtime via DI)
- **windows**: Historical branch; the GDI+ implementation was merged into `main`

## Quick Start

### Build
```bash
dotnet build

# Release build
dotnet build -c Release
```

### Tests
```bash
# No test project currently exists
```

## Architecture Patterns

### Interface-based Design
All image operations are abstracted behind interfaces (`IImageHelper`, `IImageDecoder`, `IImageQuantizer`, `IEnhancedImageHelper`, `IImageThumbnailManager`). Implementations live under `Library/Image/Skia/` (cross-platform) and `Library/Image/GdiPlus/` (Windows-only).

### Dependency Injection
Services are registered via extension methods in `ImageServiceConfiguration.cs`:
```csharp
services.AddImageServices(ImageProvider.Skia);         // standard (default)
services.AddImageServices(ImageProvider.AdvancedSkia); // advanced features
services.AddImageServices(ImageProvider.GdiPlus);      // Windows-only GDI+
```

### Provider Pattern
Three providers are available via the `ImageProvider` enum:
- **Skia**: Standard thumbnail creation with `SkiaImageHelper` (cross-platform, default)
- **AdvancedSkia**: Extended features (cropping, filtering, format conversion) with `AdvancedSkiaImageHelper` (cross-platform)
- **GdiPlus**: Windows-native implementation with `GdiPlusImageHelper` (Windows-only)

### Namespace Convention
- Root namespace: `dotnet_image_thumbnail`
- Interfaces: `dotnet_image_thumbnail.Library.Image`
- Skia implementations: `dotnet_image_thumbnail.Library.Image.Skia`
- GDI+ implementations: `dotnet_image_thumbnail.Library.Image.GdiPlus`
- Configuration: `dotnet_image_thumbnail.Library.Image.Configuration`

### Performance Conventions
- Prefer `ReadOnlySpan<T>`/`Span<T>` over allocating collections and `byte[]` copies where SkiaSharp exposes span overloads (e.g. `SKBitmap.Decode(ReadOnlySpan<byte>)`, `SKData.AsSpan()`, `File.WriteAllBytes(string, ReadOnlySpan<byte>)`).
- Avoid `stackalloc` unless the buffer is provably small; prefer `string.Create` for span-based string building.

## Supported Image Formats

PNG, JPEG, WebP, AVIF, BMP, GIF, ICO (SkiaSharp provider)

The GDI+ provider supports a subset of formats, depending on the codecs available on the host Windows installation.

## Dependencies

- **SkiaSharp** 3.119.4 - Cross-platform 2D graphics
- **Microsoft.Extensions.DependencyInjection.Abstractions** 10.0.12 - DI service registration
- **System.Drawing.Common** 10.0.12 - Windows GDI+ (GDI+ provider, Windows-only)
- **Codenet.Drawing.Common.GdiPlus** 2.0.4 - GDI+ color quantization (GDI+ provider)

## Git Workflow

## Version Control Guidelines

- **NEVER** commit changes without user approval. Ask systematically for approval before committing.
- Commit messages should be clear and follow convention:
  - ai-tooling: AI agents, automation commands, workflows, or other AI-enabled developer tooling
  - feat: New feature
  - fix: Bug fix
  - docs: Documentation
  - style: Formatting
  - refactor: Code restructuring
  - test: Adding tests
  - chore: Maintenance tasks
- **NEVER** mention AI/Claude authorship in commit messages (no "Generated with Claude Code", "AI-assisted", etc.)
