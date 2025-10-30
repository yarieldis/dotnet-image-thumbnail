namespace dotnet_image_thumbnail.Library.Image;

/// <summary>
/// Provides functionality for decoding images and determining image formats.
/// </summary>
public interface IImageDecoder
{
    /// <summary>
    /// Specifies the supported encoded image formats.
    /// </summary>
    public enum EncodedImageFormat
    {
        /// <summary>
        /// Bitmap image format (.bmp).
        /// </summary>
        Bmp,
        
        /// <summary>
        /// Graphics Interchange Format (.gif).
        /// </summary>
        Gif,
        
        /// <summary>
        /// Icon image format (.ico).
        /// </summary>
        Ico,
        
        /// <summary>
        /// JPEG image format (.jpg).
        /// </summary>
        Jpg,
        
        /// <summary>
        /// JPEG image format (.jpeg).
        /// </summary>
        Jpeg,
        
        /// <summary>
        /// Portable Network Graphics format (.png).
        /// </summary>
        Png,
        
        /// <summary>
        /// Wireless Bitmap format (.wbmp).
        /// </summary>
        Wbmp,
        
        /// <summary>
        /// WebP image format (.webp).
        /// </summary>
        Webp,
        
        /// <summary>
        /// AV1 Image File Format (.avif).
        /// </summary>
        Avif
    }

    /// <summary>
    /// Determines the encoded image format based on the file name or content.
    /// </summary>
    /// <param name="filename">The path or name of the image file to analyze.</param>
    /// <returns>
    /// The detected <see cref="EncodedImageFormat"/> if the format is supported and recognized; 
    /// otherwise, <c>null</c> if the format cannot be determined or is not supported.
    /// </returns>
    EncodedImageFormat? GetEncodedImageFormat(string filename);
}
