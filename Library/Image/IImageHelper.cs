using static dotnet_image_thumbnail.Library.Image.IImageDecoder;

namespace dotnet_image_thumbnail.Library.Image;

/// <summary>
/// Provides functionality for creating image thumbnails and saving images.
/// </summary>
public interface IImageHelper
{
    /// <summary>
    /// Creates a thumbnail image with a variable height while maintaining aspect ratio.
    /// </summary>
    /// <param name="filename">The path to the source image file.</param>
    /// <param name="imageFormat">The desired output format for the thumbnail. If <c>null</c>, the original format is used.</param>
    /// <param name="height">The target height for the thumbnail in pixels. If <c>null</c>, the original height is used.</param>
    /// <returns>A byte array containing the thumbnail image data.</returns>
    byte[] CreateThumbnailWithVariableHeight(string filename, EncodedImageFormat? imageFormat, int? height);
    
    /// <summary>
    /// Creates a thumbnail image with a variable width while maintaining aspect ratio.
    /// </summary>
    /// <param name="filename">The path to the source image file.</param>
    /// <param name="imageFormat">The desired output format for the thumbnail. If <c>null</c>, the original format is used.</param>
    /// <param name="width">The target width for the thumbnail in pixels. If <c>null</c>, the original width is used.</param>
    /// <returns>A byte array containing the thumbnail image data.</returns>
    byte[] CreateThumbnailWithVariableWidth(string filename, EncodedImageFormat? imageFormat, int? width);
    
    /// <summary>
    /// Saves image data to a file with the specified format.
    /// </summary>
    /// <param name="content">The image data as a byte array to be saved.</param>
    /// <param name="imageFormat">The format in which to save the image.</param>
    /// <param name="filename">The path where the image file will be saved.</param>
    /// <returns><c>true</c> if the image was saved successfully; otherwise, <c>false</c>.</returns>
    bool Save(byte[] content, EncodedImageFormat imageFormat, string filename);
}
