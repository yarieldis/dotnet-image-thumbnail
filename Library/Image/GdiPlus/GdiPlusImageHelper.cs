using System.Drawing;

namespace dotnet_image_thumbnail.Library.Image.GdiPlus;

/// <summary>
/// Creates thumbnails and saves images using the Windows GDI+ implementation.
/// </summary>
public class GdiPlusImageHelper : IImageHelper
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
#pragma warning disable CA1416 // Validate platform compatibility
        var loBMP = new Bitmap(filename);
#pragma warning restore CA1416 // Validate platform compatibility

#pragma warning disable CA1416 // Validate platform compatibility
        if (loBMP.Height <= height)
        {
            using MemoryStream msBmp = new();
#pragma warning disable CA1416 // Validate platform compatibility
            loBMP?.Save(msBmp, GdiPlusImageDecoder.ConvertImageFormat(imageFormat));
#pragma warning restore CA1416 // Validate platform compatibility
            return msBmp.ToArray();
        }

#pragma warning disable CA1416 // Validate platform compatibility
        decimal lnRatio = Convert.ToDecimal(loBMP.Height) / Convert.ToDecimal(height);
#pragma warning restore CA1416 // Validate platform compatibility
#pragma warning disable CA1416 // Validate platform compatibility
        int lnNewHeight = Convert.ToInt32(Convert.ToDecimal(loBMP.Height) / lnRatio);
#pragma warning restore CA1416 // Validate platform compatibility
#pragma warning disable CA1416 // Validate platform compatibility
        int lnNewWidth = Convert.ToInt32(Convert.ToDecimal(loBMP.Width) / lnRatio);
#pragma warning restore CA1416 // Validate platform compatibility

        return CreateThumbnail(loBMP, imageFormat, lnNewWidth, lnNewHeight);
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
#pragma warning disable CA1416 // Validate platform compatibility
        var loBMP = new Bitmap(filename);
#pragma warning restore CA1416 // Validate platform compatibility

#pragma warning disable CA1416 // Validate platform compatibility
        if (loBMP.Width <= width)
        {
            using MemoryStream msBmp = new();
#pragma warning disable CA1416 // Validate platform compatibility
            loBMP?.Save(msBmp, GdiPlusImageDecoder.ConvertImageFormat(imageFormat));
#pragma warning restore CA1416 // Validate platform compatibility
            return msBmp.ToArray();
        }

#pragma warning disable CA1416 // Validate platform compatibility
        decimal lnRatio = Convert.ToDecimal(loBMP.Width) / Convert.ToDecimal(width);
#pragma warning restore CA1416 // Validate platform compatibility
#pragma warning disable CA1416 // Validate platform compatibility
        int lnNewHeight = Convert.ToInt32(Convert.ToDecimal(loBMP.Height) / lnRatio);
#pragma warning restore CA1416 // Validate platform compatibility
#pragma warning disable CA1416 // Validate platform compatibility
        int lnNewWidth = Convert.ToInt32(Convert.ToDecimal(loBMP.Width) / lnRatio);
#pragma warning restore CA1416 // Validate platform compatibility

        return CreateThumbnail(loBMP, imageFormat, lnNewWidth, lnNewHeight);
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
            using var ms = new MemoryStream(content);
#pragma warning disable CA1416 // Validate platform compatibility
            var image = System.Drawing.Image.FromStream(ms);
#pragma warning restore CA1416 // Validate platform compatibility
#pragma warning disable CA1416 // Validate platform compatibility
            image.Save(filename, GdiPlusImageDecoder.ConvertImageFormat(imageFormat));
#pragma warning restore CA1416 // Validate platform compatibility
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static byte[] CreateThumbnail(Bitmap loBMP, IImageDecoder.EncodedImageFormat? imageFormat, int width, int height)
    {
        try
        {
            int newX = 0;
            int newY = 0;
#pragma warning disable CA1416 // Validate platform compatibility
            var bmpOut = new Bitmap(width, height);
#pragma warning restore CA1416 // Validate platform compatibility
#pragma warning disable CA1416 // Validate platform compatibility
            Graphics g = Graphics.FromImage(bmpOut);
#pragma warning restore CA1416 // Validate platform compatibility
#pragma warning disable CA1416 // Validate platform compatibility
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
#pragma warning restore CA1416 // Validate platform compatibility
#pragma warning disable CA1416 // Validate platform compatibility
            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
#pragma warning restore CA1416 // Validate platform compatibility
#pragma warning disable CA1416 // Validate platform compatibility
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
#pragma warning restore CA1416 // Validate platform compatibility
#pragma warning disable CA1416 // Validate platform compatibility
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
#pragma warning restore CA1416 // Validate platform compatibility
#pragma warning disable CA1416 // Validate platform compatibility
            g.DrawImage(loBMP, newX, newY, width, height);
#pragma warning restore CA1416 // Validate platform compatibility
#pragma warning disable CA1416 // Validate platform compatibility
            loBMP.Dispose();
#pragma warning restore CA1416 // Validate platform compatibility

            using MemoryStream ms = new();
#pragma warning disable CA1416 // Validate platform compatibility
            bmpOut?.Save(ms, GdiPlusImageDecoder.ConvertImageFormat(imageFormat));
#pragma warning restore CA1416 // Validate platform compatibility
            return ms.ToArray();
        }
        catch
        {
            return [];
        }
    }
}
