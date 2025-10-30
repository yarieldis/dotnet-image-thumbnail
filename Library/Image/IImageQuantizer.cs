namespace dotnet_image_thumbnail.Library.Image;

/// <summary>
/// Provides functionality for image quantization to reduce color depth and file size.
/// </summary>
public interface IImageQuantizer
{
    /// <summary>
    /// Quantizes an image to reduce the number of colors while preserving visual quality.
    /// </summary>
    /// <param name="image">The image data as a byte array to be quantized.</param>
    /// <param name="format">The format of the input image. If <c>null</c>, the format will be auto-detected.</param>
    /// <returns>A byte array containing the quantized image data with reduced color palette.</returns>
    byte[] Quantize(byte[] image, IImageDecoder.EncodedImageFormat? format);
}
