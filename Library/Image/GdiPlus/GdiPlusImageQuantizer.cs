using System.Drawing.Imaging;

namespace dotnet_image_thumbnail.Library.Image.GdiPlus;

/// <summary>
/// Quantizes images using the Windows GDI+ implementation to reduce color depth and file size.
/// </summary>
public class GdiPlusImageQuantizer : IImageQuantizer
{
    /// <summary>
    /// Quantizes an image to reduce the number of colors while preserving visual quality.
    /// </summary>
    /// <param name="image">The image data as a byte array to be quantized.</param>
    /// <param name="format">The format of the input image. If <c>null</c>, the format will be auto-detected.</param>
    /// <returns>A byte array containing the quantized image data with reduced color palette.</returns>
    public byte[] Quantize(byte[] image, IImageDecoder.EncodedImageFormat? format)
    {
        using var ms = new MemoryStream(image);
#pragma warning disable CA1416 // Validate platform compatibility
        var thumbnailImg = System.Drawing.Image.FromStream(ms);
#pragma warning restore CA1416 // Validate platform compatibility

        var quantizer = new Codenet.Drawing.Quantizers.DistinctSelection.DistinctSelectionQuantizer();
#pragma warning disable CA1416 // Validate platform compatibility
        var imageBuffer = Codenet.Drawing.Common.GdiPlusImageBuffer.FromImage(thumbnailImg, ImageLockMode.ReadOnly);
#pragma warning restore CA1416 // Validate platform compatibility
        var quantized = Codenet.Drawing.Common.GdiPlusImageBuffer.QuantizeImage(imageBuffer, quantizer, 256, 4);

        using var qms = new MemoryStream();
#pragma warning disable CA1416 // Validate platform compatibility
        quantized.Save(qms, GdiPlusImageDecoder.ConvertImageFormat(format));
#pragma warning restore CA1416 // Validate platform compatibility
        return qms.ToArray();
    }
}
