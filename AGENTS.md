# AGENTS.md - dotnet-image-thumbnail

Guidance for AI coding agents working in this repository.

## Stack

- .NET 10.0 (class library, `Microsoft.NET.Sdk`)
- C# 14 (`LangVersion` 14.0)
- SkiaSharp 3.119.4
- Microsoft.Extensions.DependencyInjection.Abstractions 10.0.12
- System.Drawing.Common 10.0.12 (Windows-only, GDI+ provider)
- Codenet.Drawing.Common.GdiPlus 2.0.4 (GDI+ quantization)

## Build

```bash
dotnet build              # Debug
dotnet build -c Release   # Release
dotnet test               # Run xUnit tests (tests/dotnet-image-thumbnail.Tests)
```

Tests live in `tests/dotnet-image-thumbnail.Tests` (xUnit). The GDI+ test is Windows-only and
self-skips on other platforms.

## Namespace Convention

- Root namespace: `dotnet_image_thumbnail`
- Interfaces: `dotnet_image_thumbnail.Library.Image`
- Skia implementations: `dotnet_image_thumbnail.Library.Image.Skia`
- GDI+ implementations: `dotnet_image_thumbnail.Library.Image.GdiPlus`
- Configuration: `dotnet_image_thumbnail.Library.Image.Configuration`

## Code Style

- Modern C# is in use: file-scoped namespaces, `using var`, collection expressions, primary constructors.
- Prefer `ReadOnlySpan<T>`/`Span<T>` over allocating collections and `byte[]` copies where SkiaSharp exposes span overloads (e.g. `SKBitmap.Decode(ReadOnlySpan<byte>)`, `SKData.AsSpan()`, `File.WriteAllBytes(string, ReadOnlySpan<byte>)`).
- Avoid `stackalloc` unless the buffer is provably small; prefer `string.Create` for span-based string building.
- Keep all image operations behind the existing interfaces (`IImageHelper`, `IImageDecoder`, `IImageQuantizer`, `IEnhancedImageHelper`, `IImageThumbnailManager`).

## Commit Conventions

- Prefixes: `feat`, `fix`, `docs`, `style`, `refactor`, `test`, `chore`, `ai-tooling`
- Do not attribute changes to an AI tool in commit messages.
