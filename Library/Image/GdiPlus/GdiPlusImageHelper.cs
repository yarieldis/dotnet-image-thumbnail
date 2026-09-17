using System.Drawing;

namespace dotnet_image_thumbnail.Library.Image.GdiPlus;

public class GdiPlusImageHelper : IImageHelper
{
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
