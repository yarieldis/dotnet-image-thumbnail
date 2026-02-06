# CLAUDE.md - dotnet-image-thumbnail

This file provides guidance to Claude Code when working with the **dotnet-image-thumbnail** repository.

## Repository Overview

**Purpose**: A .NET 8 class library for creating high-quality image thumbnails with support for multiple image formats and advanced processing capabilities using SkiaSharp.

**Status**: Active development

**Technology Stack**:
- .NET 8.0 (class library, `Microsoft.NET.Sdk`)
- SkiaSharp 3.119.1
- Microsoft.Extensions.DependencyInjection.Abstractions 8.0.2

## Project Structure

```
dotnet-image-thumbnail/
├── dotnet-image-thumbnail.sln            # Solution file
├── dotnet-image-thumbnail.csproj         # Project file (library, net8.0)
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
│       └── Skia/
│           ├── SkiaImageHelper.cs        # SkiaSharp basic image helper
│           ├── AdvancedSkiaImageHelper.cs # SkiaSharp advanced image helper
│           ├── SkiaImageDecoder.cs       # SkiaSharp image decoder
│           └── SkiaImageQuantizer.cs     # SkiaSharp color quantizer
└── README.md
```

## Branches

- **main**: Cross-platform SkiaSharp implementation
- **windows**: Windows-native GDI+ implementation using System.Drawing

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
All image operations are abstracted behind interfaces (`IImageHelper`, `IImageDecoder`, `IImageQuantizer`, `IEnhancedImageHelper`, `IImageThumbnailManager`). Implementations live under `Library/Image/Skia/`.

### Dependency Injection
Services are registered via extension methods in `ImageServiceConfiguration.cs`:
```csharp
services.AddImageServices(ImageProvider.Skia);        // standard
services.AddImageServices(ImageProvider.AdvancedSkia); // advanced features
```

### Provider Pattern
Two Skia-based providers are available:
- **Skia**: Standard thumbnail creation with `SkiaImageHelper`
- **AdvancedSkia**: Extended features (cropping, filtering, format conversion) with `AdvancedSkiaImageHelper`

### Namespace Convention
- Root namespace: `dotnet_image_thumbnail`
- Interfaces: `dotnet_image_thumbnail.Library.Image`
- Skia implementations: `dotnet_image_thumbnail.Library.Image.Skia`
- Configuration: `dotnet_image_thumbnail.Library.Image.Configuration`

## Supported Image Formats

PNG, JPEG, WebP, AVIF, BMP, GIF, ICO

## Dependencies

- **SkiaSharp** 3.119.1 - Cross-platform 2D graphics
- **Microsoft.Extensions.DependencyInjection.Abstractions** 8.0.2 - DI service registration

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
