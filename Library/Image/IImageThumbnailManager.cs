namespace dotnet_image_thumbnail.Library.Image;

/// <summary>
/// Provides functionality for managing image thumbnail file names and paths.
/// </summary>
public interface IImageThumbnailManager
{
    /// <summary>
    /// Generates a thumbnail file name based on the original file path and specified dimensions.
    /// </summary>
    /// <param name="originalFileNamePath">The path to the original image file.</param>
    /// <param name="thumbnailWidth">The width of the thumbnail in pixels. If <c>null</c>, width is not specified in the file name.</param>
    /// <param name="thumbnailHeight">The height of the thumbnail in pixels. If <c>null</c>, height is not specified in the file name.</param>
    /// <returns>A string representing the generated thumbnail file name or path.</returns>
    string RetrieveThumbnailFileName(string originalFileNamePath, int? thumbnailWidth, int? thumbnailHeight);
}
