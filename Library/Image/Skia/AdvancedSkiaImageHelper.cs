using SkiaSharp;

namespace dotnet_image_thumbnail.Library.Image.Skia;

public class AdvancedSkiaImageHelper : IEnhancedImageHelper
{
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

    public bool Save(byte[] content, IImageDecoder.EncodedImageFormat imageFormat, string filename)
    {
        try
        {
            using var inputStream = new MemoryStream(content);
            using var skBitmap = SKBitmap.Decode(inputStream);

            if (skBitmap == null)
                return false;

            var skImageFormat = SkiaImageDecoder.ConvertToSkiaImageFormat(imageFormat);

            using var image = SKImage.FromBitmap(skBitmap);
            using var data = image.Encode(skImageFormat, GetOptimalQuality(imageFormat));

            File.WriteAllBytes(filename, data.ToArray());
            return true;
        }
        catch
        {
            return false;
        }
    }

    public byte[] CreateThumbnailFromBytes(byte[] imageData, IImageDecoder.EncodedImageFormat? imageFormat, int width, int height)
    {
        using var inputStream = new MemoryStream(imageData);
        using var skBitmap = SKBitmap.Decode(inputStream) ?? throw new ArgumentException("Unable to decode image from byte array", nameof(imageData));
        return CreateHighQualityThumbnail(skBitmap, imageFormat, width, height);
    }

    public IImageDecoder.EncodedImageFormat? DetectImageFormat(byte[] imageData)
    {
        return SkiaImageDecoder.DetectImageFormat(imageData);
    }

    public byte[] ConvertImageFormat(byte[] imageData, IImageDecoder.EncodedImageFormat sourceFormat, IImageDecoder.EncodedImageFormat targetFormat)
    {
        using var inputStream = new MemoryStream(imageData);
        using var skBitmap = SKBitmap.Decode(inputStream) ?? throw new ArgumentException("Unable to decode source image", nameof(imageData));
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
                FilterQuality = SKFilterQuality.High,
                IsDither = true
            };

            // Calculate the destination rectangle to maintain aspect ratio
            var sourceRect = new SKRect(0, 0, originalBitmap.Width, originalBitmap.Height);
            var destRect = CalculateDestinationRect(originalBitmap.Width, originalBitmap.Height, width, height);

            // Draw the resized image with high quality
            canvas.DrawBitmap(originalBitmap, sourceRect, destRect, paint);

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

    public byte[] ApplyImageFilters(byte[] imageData, IImageDecoder.EncodedImageFormat? format,
        float brightness = 1.0f, float contrast = 1.0f, float saturation = 1.0f)
    {
        using var inputStream = new MemoryStream(imageData);
        using var skBitmap = SKBitmap.Decode(inputStream);

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