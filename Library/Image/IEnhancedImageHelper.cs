using static dotnet_image_thumbnail.Library.Image.IImageDecoder;

namespace dotnet_image_thumbnail.Library.Image;

/// <summary>
/// Enhanced image helper interface with additional functionality
/// </summary>
public interface IEnhancedImageHelper : IImageHelper
{
    /// <summary>
    /// Creates a thumbnail with cropping to maintain exact dimensions
    /// </summary>
    byte[] CreateThumbnailWithCrop(string filename, EncodedImageFormat? imageFormat, int width, int height);

    /// <summary>
    /// Applies image filters (brightness, contrast, saturation)
    /// </summary>
    byte[] ApplyImageFilters(byte[] imageData, EncodedImageFormat? format, 
        float brightness = 1.0f, float contrast = 1.0f, float saturation = 1.0f);

    /// <summary>
    /// Creates a thumbnail from byte array instead of file
    /// </summary>
    byte[] CreateThumbnailFromBytes(byte[] imageData, EncodedImageFormat? imageFormat, int width, int height);

    /// <summary>
    /// Detects the format of an image from its byte content
    /// </summary>
    EncodedImageFormat? DetectImageFormat(byte[] imageData);

    /// <summary>
    /// Converts image from one format to another
    /// </summary>
    byte[] ConvertImageFormat(byte[] imageData, EncodedImageFormat sourceFormat, EncodedImageFormat targetFormat);
}