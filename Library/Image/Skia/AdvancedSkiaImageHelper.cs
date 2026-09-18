using SkiaSharp;

namespace dotnet_image_thumbnail.Library.Image.Skia;

/// <summary>
/// Creates thumbnails and applies advanced image operations using the SkiaSharp implementation.
/// </summary>
public class AdvancedSkiaImageHelper : IEnhancedImageHelper
{
    /// <summary>
    /// Creates a thumbnail image with a variable height while maintaining aspect ratio.
    /// </summary>
    /// <param name="filename">The path to the source image file.</param>
    /// <param name="imageFormat">The desired output format for the thumbnail. If <c>null</c>, the original format is used.</param>
    /// <param name="height">The target height for the thumbnail in pixels. If <c>null</c>, the original height is used.</param>
    /// <returns>A byte array containing the thumbnail image data.</returns>
    public byte[] CreateThumbnailWithVariableHeight(string filename, IImageDecoder.EncodedImageFormat? imageFormat, int? height)
    {
        using var skBitmap = SKBitmap.Decode(filename) ?? throw new ArgumentException("Unable to decode image from file", nameof(filename));
        if (skBitmap.Height <= height)
        {
            return SaveBitmapToByteArray(skBitmap, imageFormat);
        }

        int iHeight = height ?? skBitmap.Height;
        decimal ratio = (decimal)skBitmap.Height / iHeight;
        int newHeight = (int)(skBitmap.Height / ratio);
        int newWidth = (int)(skBitmap.Width / ratio);

        return CreateHighQualityThumbnail(skBitmap, imageFormat, newWidth, newHeight);
    }

    /// <summary>
    /// Creates a thumbnail image with a variable width while maintaining aspect ratio.
    /// </summary>
    /// <param name="filename">The path to the source image file.</param>
    /// <param name="imageFormat">The desired output format for the thumbnail. If <c>null</c>, the original format is used.</param>
    /// <param name="width">The target width for the thumbnail in pixels. If <c>null</c>, the original width is used.</param>
    /// <returns>A byte array containing the thumbnail image data.</returns>
    public byte[] CreateThumbnailWithVariableWidth(string filename, IImageDecoder.EncodedImageFormat? imageFormat, int? width)
    {
        using var skBitmap = SKBitmap.Decode(filename) ?? throw new ArgumentException("Unable to decode image from file", nameof(filename));
        if (skBitmap.Width <= width)
        {
            return SaveBitmapToByteArray(skBitmap, imageFormat);
        }

        int iWidth = width ?? skBitmap.Width;
        decimal ratio = (decimal)skBitmap.Width / iWidth;
        int newHeight = (int)(skBitmap.Height / ratio);
        int newWidth = (int)(skBitmap.Width / ratio);

        return CreateHighQualityThumbnail(skBitmap, imageFormat, newWidth, newHeight);
    }

    /// <summary>
    /// Saves image data to a file with the specified format.
    /// </summary>
    /// <param name="content">The image data as a byte array to be saved.</param>
    /// <param name="imageFormat">The format in which to save the image.</param>
    /// <param name="filename">The path where the image file will be saved.</param>
    /// <returns><c>true</c> if the image was saved successfully; otherwise, <c>false</c>.</returns>
    public bool Save(byte[] content, IImageDecoder.EncodedImageFormat imageFormat, string filename)
    {
        try
        {
            using var skBitmap = SKBitmap.Decode(content);

            if (skBitmap == null)
                return false;

            var skImageFormat = SkiaImageDecoder.ConvertToSkiaImageFormat(imageFormat);

            using var image = SKImage.FromBitmap(skBitmap);
            using var data = image.Encode(skImageFormat, GetOptimalQuality(imageFormat));

            File.WriteAllBytes(filename, data.AsSpan());
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Creates a thumbnail from a byte array instead of a file.
    /// </summary>
    /// <param name="imageData">The image data as a byte array.</param>
    /// <param name="imageFormat">The desired output format for the thumbnail.</param>
    /// <param name="width">The target width for the thumbnail in pixels.</param>
    /// <param name="height">The target height for the thumbnail in pixels.</param>
    /// <returns>A byte array containing the thumbnail image data.</returns>
    public byte[] CreateThumbnailFromBytes(byte[] imageData, IImageDecoder.EncodedImageFormat? imageFormat, int width, int height)
    {
        using var skBitmap = SKBitmap.Decode(imageData) ?? throw new ArgumentException("Unable to decode image from byte array", nameof(imageData));
        return CreateHighQualityThumbnail(skBitmap, imageFormat, width, height);
    }

    /// <summary>
    /// Detects the format of an image from its byte content.
    /// </summary>
    /// <param name="imageData">The image data as a byte array to analyze.</param>
    /// <returns>The detected format if recognized; otherwise, <c>null</c>.</returns>
    public IImageDecoder.EncodedImageFormat? DetectImageFormat(byte[] imageData)
    {
        return SkiaImageDecoder.DetectImageFormat(imageData);
    }

    /// <summary>
    /// Converts an image from one format to another.
    /// </summary>
    /// <param name="imageData">The image data as a byte array.</param>
    /// <param name="sourceFormat">The current format of the image.</param>
    /// <param name="targetFormat">The format to which the image is converted.</param>
    /// <returns>A byte array containing the converted image data.</returns>
    public byte[] ConvertImageFormat(byte[] imageData, IImageDecoder.EncodedImageFormat sourceFormat, IImageDecoder.EncodedImageFormat targetFormat)
    {
        using var skBitmap = SKBitmap.Decode(imageData) ?? throw new ArgumentException("Unable to decode source image", nameof(imageData));
        using var image = SKImage.FromBitmap(skBitmap);

        var targetSkFormat = SkiaImageDecoder.ConvertToSkiaImageFormat(targetFormat);

        using var data = image.Encode(targetSkFormat, GetOptimalQuality(targetFormat));
        return data.ToArray();
    }

    private static byte[] CreateHighQualityThumbnail(SKBitmap originalBitmap, IImageDecoder.EncodedImageFormat? imageFormat, int width, int height)
    {
        try
        {
            // Create a surface for high-quality rendering
            var info = new SKImageInfo(width, height, SKColorType.Rgba8888, SKAlphaType.Premul);
            using var surface = SKSurface.Create(info);
            using var canvas = surface.Canvas;

            canvas.Clear(SKColors.White);

            // Configure high-quality paint
            using var paint = new SKPaint
            {
                IsAntialias = true,
                IsDither = true
            };

            // Calculate the destination rectangle to maintain aspect ratio
            var sourceRect = new SKRect(0, 0, originalBitmap.Width, originalBitmap.Height);
            var destRect = CalculateDestinationRect(originalBitmap.Width, originalBitmap.Height, width, height);

            // Draw the resized image with high quality sampling
            using var sourceImage = SKImage.FromBitmap(originalBitmap);
            canvas.DrawImage(sourceImage, sourceRect, destRect, new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.Linear), paint);

            // Get the resulting image
            using var image = surface.Snapshot();
            var skImageFormat = SkiaImageDecoder.ConvertToSkiaImageFormat(imageFormat);

            using var data = image.Encode(skImageFormat, GetOptimalQuality(imageFormat));
            return data.ToArray();
        }
        catch
        {
            return [];
        }
    }

    private static byte[] SaveBitmapToByteArray(SKBitmap bitmap, IImageDecoder.EncodedImageFormat? imageFormat)
    {
        using var image = SKImage.FromBitmap(bitmap);
        var skImageFormat = SkiaImageDecoder.ConvertToSkiaImageFormat(imageFormat);

        using var data = image.Encode(skImageFormat, GetOptimalQuality(imageFormat));
        return data.ToArray();
    }

    private static SKRect CalculateDestinationRect(int sourceWidth, int sourceHeight, int targetWidth, int targetHeight)
    {
        // Calculate scaling to fit within target dimensions while maintaining aspect ratio
        float scaleX = (float)targetWidth / sourceWidth;
        float scaleY = (float)targetHeight / sourceHeight;
        float scale = Math.Min(scaleX, scaleY);

        float scaledWidth = sourceWidth * scale;
        float scaledHeight = sourceHeight * scale;

        // Center the image
        float x = (targetWidth - scaledWidth) / 2;
        float y = (targetHeight - scaledHeight) / 2;

        return new SKRect(x, y, x + scaledWidth, y + scaledHeight);
    }

    private static int GetOptimalQuality(IImageDecoder.EncodedImageFormat? format)
    {
        return format switch
        {
            IImageDecoder.EncodedImageFormat.Jpeg => 85,
            IImageDecoder.EncodedImageFormat.Jpg => 85,
            IImageDecoder.EncodedImageFormat.Webp => 80,
            IImageDecoder.EncodedImageFormat.Avif => 75,
            _ => 90 // For lossless formats like PNG, quality parameter is ignored
        };
    }

    /// <summary>
    /// Creates a thumbnail with cropping to maintain exact dimensions.
    /// </summary>
    /// <param name="filename">The path to the source image file.</param>
    /// <param name="imageFormat">The desired output format for the thumbnail.</param>
    /// <param name="width">The target width for the thumbnail in pixels.</param>
    /// <param name="height">The target height for the thumbnail in pixels.</param>
    /// <returns>A byte array containing the thumbnail image data.</returns>
    public byte[] CreateThumbnailWithCrop(string filename, IImageDecoder.EncodedImageFormat? imageFormat, int width, int height)
    {
        using var skBitmap = SKBitmap.Decode(filename) ?? throw new ArgumentException("Unable to decode image from file", nameof(filename));

        // Calculate crop rectangle to maintain aspect ratio
        var sourceAspect = (float)skBitmap.Width / skBitmap.Height;
        var targetAspect = (float)width / height;

        SKRect cropRect;
        if (sourceAspect > targetAspect)
        {
            // Source is wider, crop width
            var cropWidth = (int)(skBitmap.Height * targetAspect);
            var cropX = (skBitmap.Width - cropWidth) / 2;
            cropRect = new SKRect(cropX, 0, cropX + cropWidth, skBitmap.Height);
        }
        else
        {
            // Source is taller, crop height
            var cropHeight = (int)(skBitmap.Width / targetAspect);
            var cropY = (skBitmap.Height - cropHeight) / 2;
            cropRect = new SKRect(0, cropY, skBitmap.Width, cropY + cropHeight);
        }

        // Create cropped bitmap
        using var croppedBitmap = new SKBitmap((int)cropRect.Width, (int)cropRect.Height);
        using var canvas = new SKCanvas(croppedBitmap);

        var sourceRect = cropRect;
        var destRect = new SKRect(0, 0, cropRect.Width, cropRect.Height);

        canvas.DrawBitmap(skBitmap, sourceRect, destRect);

        // Now resize to final dimensions
        return CreateHighQualityThumbnail(croppedBitmap, imageFormat, width, height);
    }

    /// <summary>
    /// Applies image filters (brightness, contrast, and saturation).
    /// </summary>
    /// <param name="imageData">The image data as a byte array.</param>
    /// <param name="format">The desired output format for the filtered image.</param>
    /// <param name="brightness">The brightness multiplier. Defaults to <c>1.0f</c>.</param>
    /// <param name="contrast">The contrast multiplier. Defaults to <c>1.0f</c>.</param>
    /// <param name="saturation">The saturation multiplier. Defaults to <c>1.0f</c>.</param>
    /// <returns>A byte array containing the filtered image data.</returns>
    public byte[] ApplyImageFilters(byte[] imageData, IImageDecoder.EncodedImageFormat? format,
        float brightness = 1.0f, float contrast = 1.0f, float saturation = 1.0f)
    {
        using var skBitmap = SKBitmap.Decode(imageData);

        if (skBitmap == null)
            return imageData;

        var info = new SKImageInfo(skBitmap.Width, skBitmap.Height, SKColorType.Rgba8888, SKAlphaType.Premul);
        using var surface = SKSurface.Create(info);
        using var canvas = surface.Canvas;

        // Create color filter for brightness, contrast, and saturation
        var colorMatrix = new float[]
        {
            contrast, 0, 0, 0, brightness - 1,  // Red
            0, contrast, 0, 0, brightness - 1,  // Green  
            0, 0, contrast, 0, brightness - 1,  // Blue
            0, 0, 0, 1, 0                       // Alpha
        };

        using var colorFilter = SKColorFilter.CreateColorMatrix(colorMatrix);
        using var paint = new SKPaint { ColorFilter = colorFilter };

        canvas.DrawBitmap(skBitmap, 0, 0, paint);

        using var image = surface.Snapshot();
        var skImageFormat = SkiaImageDecoder.ConvertToSkiaImageFormat(format);

        using var data = image.Encode(skImageFormat, GetOptimalQuality(format));
        return data.ToArray();
    }
}