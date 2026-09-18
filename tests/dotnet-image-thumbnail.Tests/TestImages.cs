using SkiaSharp;

namespace dotnet_image_thumbnail.Tests;

/// <summary>
/// Helpers for generating in-memory and on-disk test images.
/// </summary>
internal static class TestImages
{
    public static byte[] CreatePng(int width, int height, SKColor? color = null)
    {
        using var bitmap = new SKBitmap(width, height, SKColorType.Rgba8888, SKAlphaType.Premul);
        bitmap.Erase(color ?? SKColors.SteelBlue);

        using var image = SKImage.FromBitmap(bitmap);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        return data.ToArray();
    }

    public static string CreateTempPngFile(int width, int height)
    {
        string path = Path.Combine(Path.GetTempPath(), $"dit-{Guid.NewGuid():N}.png");
        File.WriteAllBytes(path, CreatePng(width, height));
        return path;
    }
}
