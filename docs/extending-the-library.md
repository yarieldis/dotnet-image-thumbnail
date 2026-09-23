# Extending the Library: Implementing Your Own Image Providers

This guide explains the **contracts** behind the library's core interfaces and shows
how to provide your own implementations — for example, to back the library with a
different imaging engine (ImageMagick, ImageSharp, a cloud service, hardware codecs,
etc.) — and how to wire them into your application's dependency injection container.

## Table of Contents

- [How the pieces fit together](#how-the-pieces-fit-together)
- [The contracts](#the-contracts)
  - [`IImageDecoder`](#iimagedecoder)
  - [`IImageHelper`](#iimagehelper)
  - [`IImageQuantizer`](#iimagequantizer)
  - [`IEnhancedImageHelper`](#ienhancedimagehelper)
- [Registering your implementations](#registering-your-implementations)
- [Using the services in your solution](#using-the-services-in-your-solution)
- [Complete example](#complete-example)
- [Things to watch out for](#things-to-watch-out-for)

## How the pieces fit together

All image work flows through a small set of interfaces in the
`dotnet_image_thumbnail.Library.Image` namespace:

```text
                    ┌────────────────────────────┐
                    │   IImageThumbnailManager   │  ← high-level entry point
                    │  (naming + caching)        │
                    └─────────────┬──────────────┘
                                  │ depends on
              ┌───────────────────┼───────────────────┐
              ▼                   ▼                   ▼
      ┌──────────────┐    ┌──────────────┐    ┌──────────────┐
      │ IImageHelper │    │ IImageDecoder│    │IImageQuantizer│
      │ (resize/save)│    │(format detect)│    │ (reduce colors)│
      └──────────────┘    └──────────────┘    └──────────────┘

              ┌────────────────────┐
              │ IEnhancedImageHelper│  ← optional, adds advanced ops
              └────────────────────┘      (inherits IImageHelper)
```

- `IImageThumbnailManager` orchestrates the other three: it asks the decoder for the
  format, the helper to produce thumbnail bytes, and the quantizer to optimize them.
- `IEnhancedImageHelper` extends `IImageHelper` with advanced operations but is **not**
  consumed by `IImageThumbnailManager`; you use it directly in your own code.

Because every component depends only on these abstractions, you can swap any (or all)
of them for your own implementations without touching the rest of the library.

## The contracts

All interfaces live in `dotnet_image_thumbnail.Library.Image`.

### `IImageDecoder`

Responsible for detecting the encoded format of an image from its file path or content.

```csharp
public interface IImageDecoder
{
    public enum EncodedImageFormat
    {
        Bmp, Gif, Ico, Jpg, Jpeg, Png, Wbmp, Webp, Avif
    }

    EncodedImageFormat? GetEncodedImageFormat(string filename);
}
```

**What to implement**

- Return the detected `EncodedImageFormat`, or `null` when the format is unknown or
  unsupported.
- The `filename` argument is a path. You may decide by extension, by reading magic
  bytes from the file header, or both — but remember it may also be a path to a file
  that does not exist yet (see the note about `Save` below).

**Example**

```csharp
using dotnet_image_thumbnail.Library.Image;

public sealed class MyImageDecoder : IImageDecoder
{
    public IImageDecoder.EncodedImageFormat? GetEncodedImageFormat(string filename)
    {
        // Prefer content sniffing over extension for reliability.
        return Path.GetExtension(filename).ToLowerInvariant() switch
        {
            ".png"  => IImageDecoder.EncodedImageFormat.Png,
            ".gif"  => IImageDecoder.EncodedImageFormat.Gif,
            ".jpg"  => IImageDecoder.EncodedImageFormat.Jpeg,
            ".jpeg" => IImageDecoder.EncodedImageFormat.Jpeg,
            ".bmp"  => IImageDecoder.EncodedImageFormat.Bmp,
            ".webp" => IImageDecoder.EncodedImageFormat.Webp,
            ".avif" => IImageDecoder.EncodedImageFormat.Avif,
            ".ico"  => IImageDecoder.EncodedImageFormat.Ico,
            _       => null
        };
    }
}
```

### `IImageHelper`

Performs the actual thumbnail creation and file saving.

```csharp
public interface IImageHelper
{
    byte[] CreateThumbnailWithVariableHeight(string filename, EncodedImageFormat? imageFormat, int? height);
    byte[] CreateThumbnailWithVariableWidth(string filename, EncodedImageFormat? imageFormat, int? width);
    bool Save(byte[] content, EncodedImageFormat imageFormat, string filename);
}
```

**What to implement**

- The two `CreateThumbnail*` methods take a source file and a single target dimension,
  and must **preserve the aspect ratio** (the other dimension is derived from the
  source). Return the encoded thumbnail bytes.
- `imageFormat` is the desired output format; treat `null` as "keep the source format".
- `height`/`width` of `null` means "keep the source dimension" — do not pass it through
  unhandled.
- `Save` writes `content` to `filename` in the given format and returns whether the
  write succeeded.

**Example**

```csharp
using static dotnet_image_thumbnail.Library.Image.IImageDecoder;

public sealed class MyImageHelper : IImageHelper
{
    public byte[] CreateThumbnailWithVariableHeight(string filename, EncodedImageFormat? imageFormat, int? height)
        => CreateThumbnail(filename, imageFormat, width: null, height);

    public byte[] CreateThumbnailWithVariableWidth(string filename, EncodedImageFormat? imageFormat, int? width)
        => CreateThumbnail(filename, imageFormat, width, height: null);

    private static byte[] CreateThumbnail(string filename, EncodedImageFormat? imageFormat, int? width, int? height)
    {
        // Use your imaging engine here. This is where the real work happens.
        // 1. Decode `filename`.
        // 2. Scale to `width` or `height`, keeping aspect ratio.
        // 3. Encode to `imageFormat` (or the source format when null).
        throw new NotImplementedException();
    }

    public bool Save(byte[] content, EncodedImageFormat imageFormat, string filename)
    {
        // Encode `content` into `imageFormat` and write to `filename`.
        throw new NotImplementedException();
    }
}
```

> `using static dotnet_image_thumbnail.Library.Image.IImageDecoder;` lets you refer to
> `EncodedImageFormat` unqualified, matching the built-in implementations.

### `IImageQuantizer`

Reduces the number of colors in an image (palette quantization) to shrink file size,
primarily for palette-based formats such as GIF and PNG.

```csharp
public interface IImageQuantizer
{
    byte[] Quantize(byte[] image, IImageDecoder.EncodedImageFormat? format);
}
```

**What to implement**

- Accept encoded image bytes and return new encoded bytes with a reduced color palette.
- `format` is the input format; treat `null` as "auto-detect".
- The result must remain a valid image of the same format.

**Example**

```csharp
public sealed class MyImageQuantizer : IImageQuantizer
{
    public byte[] Quantize(byte[] image, IImageDecoder.EncodedImageFormat? format)
    {
        // Decode `image`, apply color reduction, and re-encode.
        throw new NotImplementedException();
    }
}
```

### `IEnhancedImageHelper`

An optional superset of `IImageHelper` with advanced operations. It **inherits** the
three `IImageHelper` members, so an implementation must provide those too.

```csharp
public interface IEnhancedImageHelper : IImageHelper
{
    byte[] CreateThumbnailWithCrop(string filename, EncodedImageFormat? imageFormat, int width, int height);
    byte[] ApplyImageFilters(byte[] imageData, EncodedImageFormat? format,
        float brightness = 1.0f, float contrast = 1.0f, float saturation = 1.0f);
    byte[] CreateThumbnailFromBytes(byte[] imageData, EncodedImageFormat? imageFormat, int width, int height);
    EncodedImageFormat? DetectImageFormat(byte[] imageData);
    byte[] ConvertImageFormat(byte[] imageData, EncodedImageFormat sourceFormat, EncodedImageFormat targetFormat);
}
```

**What to implement**

- `CreateThumbnailWithCrop` — produce an exact `width × height` image, cropping
  (not stretching) to fill the dimensions.
- `ApplyImageFilters` — adjust brightness/contrast/saturation. `1.0f` is the neutral
  value for each.
- `CreateThumbnailFromBytes` — like `CreateThumbnail*`, but the source is in-memory
  bytes instead of a file path.
- `DetectImageFormat` — sniff the format from raw bytes (no file involved).
- `ConvertImageFormat` — transcode between two known formats.

**Example**

```csharp
public sealed class MyEnhancedImageHelper : IEnhancedImageHelper
{
    public byte[] CreateThumbnailWithVariableHeight(string filename, EncodedImageFormat? imageFormat, int? height)
        => throw new NotImplementedException();

    public byte[] CreateThumbnailWithVariableWidth(string filename, EncodedImageFormat? imageFormat, int? width)
        => throw new NotImplementedException();

    public bool Save(byte[] content, EncodedImageFormat imageFormat, string filename)
        => throw new NotImplementedException();

    public byte[] CreateThumbnailWithCrop(string filename, EncodedImageFormat? imageFormat, int width, int height)
        => throw new NotImplementedException();

    public byte[] ApplyImageFilters(byte[] imageData, EncodedImageFormat? format,
        float brightness = 1.0f, float contrast = 1.0f, float saturation = 1.0f)
        => throw new NotImplementedException();

    public byte[] CreateThumbnailFromBytes(byte[] imageData, EncodedImageFormat? imageFormat, int width, int height)
        => throw new NotImplementedException();

    public EncodedImageFormat? DetectImageFormat(byte[] imageData)
        => throw new NotImplementedException();

    public byte[] ConvertImageFormat(byte[] imageData, EncodedImageFormat sourceFormat, EncodedImageFormat targetFormat)
        => throw new NotImplementedException();
}
```

## Registering your implementations

Registration happens through the extension method
`AddImageServices` in `dotnet_image_thumbnail.Library.Image.Configuration`.

The library registers its built-in implementations with `TryAddScoped`, which only
adds a registration when the service type is **not already registered**. That gives
you a clean extension point: register your own implementations *first*, then call
`AddImageServices` — your versions win, and any pieces you did not override are filled
in by the library.

### Option A — override some or all pieces (recommended)

```csharp
using dotnet_image_thumbnail.Library.Image;
using dotnet_image_thumbnail.Library.Image.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

services.TryAddScoped<IImageDecoder, MyImageDecoder>();
services.TryAddScoped<IImageHelper, MyImageHelper>();
services.TryAddScoped<IImageQuantizer, MyImageQuantizer>();

// Optional: expose the advanced surface too.
services.TryAddScoped<IEnhancedImageHelper, MyEnhancedImageHelper>();

// Registers IImageThumbnailManager and any missing built-in pieces.
// Your pre-registered implementations are preserved.
services.AddImageServices();
```

You can override just one interface — e.g. only `IImageHelper` — and let the library
supply the decoder and quantizer. Everything is mixed and matched through the
abstractions.

### Option B — register everything manually

Skip `AddImageServices` entirely and register the full graph yourself:

```csharp
services.TryAddScoped<IImageDecoder, MyImageDecoder>();
services.TryAddScoped<IImageHelper, MyImageHelper>();
services.TryAddScoped<IImageQuantizer, MyImageQuantizer>();
services.TryAddScoped<IEnhancedImageHelper, MyEnhancedImageHelper>();
services.TryAddScoped<IImageThumbnailManager, ImageThumbnailManager>();
```

> If you register `IEnhancedImageHelper`, remember that `IImageThumbnailManager`
> depends on the **base** `IImageHelper`. To have the manager use your enhanced
> implementation, register it for `IImageHelper` as well (or instead).

### Lifetime guidance

- The built-in services are registered as **scoped**. Match that unless you have a
  reason not to.
- Prefer **stateless, thread-safe** implementations. Do not store per-request state in
  the implementations themselves, or a scoped/singleton instance may leak state
  between requests.

## Using the services in your solution

Inject the high-level manager and let it handle naming, caching, and regeneration:

```csharp
public sealed class ThumbnailService
{
    private readonly IImageThumbnailManager _manager;

    public ThumbnailService(IImageThumbnailManager manager) => _manager = manager;

    public string GetThumbnail(string imagePath)
        => _manager.RetrieveThumbnailFileName(imagePath, thumbnailWidth: 300, thumbnailHeight: null);
}
```

Or inject individual interfaces when you need finer control:

```csharp
public sealed class ImageService
{
    private readonly IImageDecoder _decoder;
    private readonly IImageHelper _helper;
    private readonly IEnhancedImageHelper _enhanced;

    public ImageService(IImageDecoder decoder, IImageHelper helper, IEnhancedImageHelper enhanced)
    {
        _decoder = decoder;
        _helper = helper;
        _enhanced = enhanced;
    }

    public byte[] Crop(string path, int width, int height)
    {
        var format = _decoder.GetEncodedImageFormat(path);
        return _enhanced.CreateThumbnailWithCrop(path, format, width, height);
    }
}
```

## Complete example

A minimal but complete wiring showing a custom engine being plugged in:

```csharp
using dotnet_image_thumbnail.Library.Image;
using dotnet_image_thumbnail.Library.Image.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

var services = new ServiceCollection();

// 1. Your implementations first (they win over the library's TryAddScoped).
services.TryAddScoped<IImageDecoder, MyImageDecoder>();
services.TryAddScoped<IImageHelper, MyImageHelper>();
services.TryAddScoped<IImageQuantizer, MyImageQuantizer>();
services.TryAddScoped<IEnhancedImageHelper, MyEnhancedImageHelper>();

// 2. Library registrations fill in the manager (and anything you skipped).
services.AddImageServices();

await using var provider = services.BuildServiceProvider();

// 3. Use it.
var manager = provider.GetRequiredService<IImageThumbnailManager>();
string thumbnail = manager.RetrieveThumbnailFileName("photos/beach.jpg", 320, null);
```

## Things to watch out for

- **`null` format arguments.** `imageFormat`/`format` of `null` means "detect or keep
  the original format". Handle it explicitly rather than passing it to your engine.
- **`Save` may target a path that does not exist yet.** `IImageThumbnailManager` can
  call `Save` on a not-yet-created thumbnail path; a decoder that only reads file
  content must not assume the file exists when called for the destination.
- **Aspect ratio.** `CreateThumbnailWithVariableWidth/Height` must preserve the source
  aspect ratio; only `CreateThumbnailWithCrop` should produce exact dimensions by
  cropping.
- **Encoded bytes in, encoded bytes out.** The helper and quantizer work on encoded
  `byte[]`, not on engine-specific image objects. Do your decode/encode internally.
- **Thread safety.** Assume scoped lifetimes and concurrent use; keep implementations
  stateless or synchronize any shared resources.
- **Return values.** `Save` should return `false` on failure rather than throwing,
  where possible; `GetEncodedImageFormat` returns `null` for unknown/unsupported
  formats.
