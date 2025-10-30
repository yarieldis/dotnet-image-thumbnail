using System.Drawing.Imaging;

namespace dotnet_image_thumbnail.Library.Image.GdiPlus;

public class GdiPlusImageQuantizer : IImageQuantizer
{
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
