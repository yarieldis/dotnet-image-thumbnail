namespace dotnet_image_thumbnail.Library.Image;

public class ImageThumbnailManager(IImageQuantizer imageQuantizer, IImageDecoder imageDecoder, IImageHelper imageHelper) : IImageThumbnailManager
{
    public string RetrieveThumbnailFileName(string originalFileNamePath, int? thumbnailWidth, int? thumbnailHeight)
    {
        if (string.IsNullOrEmpty(originalFileNamePath))
            return string.Empty;

        // set the name and format of the thumbail
        string? imageFolderPath = Path.GetDirectoryName(originalFileNamePath);
        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(originalFileNamePath);
        ReadOnlySpan<char> fileNameExtension = Path.GetExtension(originalFileNamePath).AsSpan();
        string thumbnailExtension = fileNameExtension.Equals(".gif", StringComparison.OrdinalIgnoreCase) ? ".gif"
            : fileNameExtension.Equals(".png", StringComparison.OrdinalIgnoreCase) ? ".png"
            : ".jpeg";

        int width = thumbnailWidth ?? 0;
        int height = thumbnailHeight ?? 0;
        string thumbnailFileName = $"Thumbnail_{fileNameWithoutExtension}_{width}x{height}{thumbnailExtension}";

        if (string.IsNullOrEmpty(imageFolderPath))
            return string.Empty;

        string? thumbnailFileFullPath = Path.Combine(imageFolderPath, thumbnailFileName);

        try
        {
            var imageFormat = imageDecoder.GetEncodedImageFormat(originalFileNamePath);
            if (!File.Exists(thumbnailFileFullPath))
            {
                var thumbImg = CreateThumbnailBytes(originalFileNamePath, imageFormat, thumbnailWidth, thumbnailHeight);
                if (thumbImg == null)
                    return string.Empty;
                // save the thumbail
                SaveCustomThumbnailImage(thumbImg, imageFolderPath, thumbnailFileName);
            }
            else
            {
                // verify if the original file has a date of modification after the date of creation of thumnbail
                var thumbnailFile = new FileInfo(Path.Combine(imageFolderPath, thumbnailFileName));
                var originalFile = new FileInfo(originalFileNamePath);
                if (DateTime.Compare(originalFile.LastWriteTime, thumbnailFile.LastWriteTime) >= 0)
                {
                    // delete thumbnail
                    File.Delete(thumbnailFileFullPath);
                    // create new thumbail
                    var thumbImg = CreateThumbnailBytes(originalFileNamePath, imageFormat, thumbnailWidth, thumbnailHeight);
                    if (thumbImg == null)
                        return string.Empty;
                    // save the thumbail
                    SaveCustomThumbnailImage(thumbImg, imageFolderPath, thumbnailFileName);
                }
            }

            // return the thumbnail file name
            return FormatForUrl(thumbnailFileName);
        }
        catch (Exception)
        {
            // log.Error("Error when retrieving thumbail", ex);
            // forward the error 
            throw;
        }
    }

    private void SaveCustomThumbnailImage(byte[] thumbnailImg, string thumbnailFolderPath, string thumbnailFileName)
    {
        var thumbnailCodec = imageDecoder.GetEncodedImageFormat(Path.Combine(thumbnailFolderPath, thumbnailFileName));
        if (thumbnailCodec == IImageDecoder.EncodedImageFormat.Gif || thumbnailCodec == IImageDecoder.EncodedImageFormat.Png)
        {
            var quantized = imageQuantizer.Quantize(thumbnailImg, thumbnailCodec);
            imageHelper.Save(quantized, (IImageDecoder.EncodedImageFormat)thumbnailCodec, Path.Combine(thumbnailFolderPath, thumbnailFileName));
        }
        if (thumbnailCodec == IImageDecoder.EncodedImageFormat.Jpeg)
        {
            imageHelper.Save(thumbnailImg, (IImageDecoder.EncodedImageFormat)thumbnailCodec, Path.Combine(thumbnailFolderPath, thumbnailFileName));
        }
    }

    private byte[] CreateThumbnailBytes(string originalFileNamePath, IImageDecoder.EncodedImageFormat? imageFormat, int? thumbnailWidth, int? thumbnailHeight)
    {
        if (thumbnailWidth.HasValue)
            return imageHelper.CreateThumbnailWithVariableHeight(originalFileNamePath, imageFormat, thumbnailWidth);

        if (thumbnailHeight.HasValue)
            return imageHelper.CreateThumbnailWithVariableWidth(originalFileNamePath, imageFormat, thumbnailHeight);

        throw new Exception("No width or height are defined");
    }

    #region "        Format For URL"
    private static string FormatForUrl(string text)
    {
        ReadOnlySpan<char> source = text.AsSpan();

        int extraLength = 0;
        foreach (char c in source)
        {
            if (c is '%' or '"' or '#' or '&' or '\'')
                extraLength += 2;
        }

        if (extraLength == 0)
            return text;

        return string.Create(source.Length + extraLength, text, static (destination, value) =>
        {
            ReadOnlySpan<char> input = value.AsSpan();
            int index = 0;
            foreach (char c in input)
            {
                switch (c)
                {
                    case '%': "%25".AsSpan().CopyTo(destination[index..]); index += 3; break;
                    case '"': "%22".AsSpan().CopyTo(destination[index..]); index += 3; break;
                    case '#': "%23".AsSpan().CopyTo(destination[index..]); index += 3; break;
                    case '&': "%26".AsSpan().CopyTo(destination[index..]); index += 3; break;
                    case '\'': "%27".AsSpan().CopyTo(destination[index..]); index += 3; break;
                    default: destination[index++] = c; break;
                }
            }
        });
    }
    #endregion
}
