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
        string fileNameExtension = Path.GetExtension(originalFileNamePath);
        string thumbnailFileNameTemplate = fileNameExtension.ToLower() switch
        {
            ".gif" => "Thumbnail_{0}_{1}x{2}.gif",
            ".png" => "Thumbnail_{0}_{1}x{2}.png",
            _ => "Thumbnail_{0}_{1}x{2}.jpeg",
        };
        string thumbnailFileName = string.Format(thumbnailFileNameTemplate, fileNameWithoutExtension, thumbnailWidth.HasValue ? thumbnailWidth : 0, thumbnailHeight.HasValue ? thumbnailHeight : 0);

        if (string.IsNullOrEmpty(imageFolderPath))
            return string.Empty;

        string? thumbnailFileFullPath = Path.Combine(imageFolderPath, thumbnailFileName);

        try
        {
            var imageFormat = imageDecoder.GetEncodedImageFormat(originalFileNamePath);
            if (!File.Exists(thumbnailFileFullPath))
            {
                byte[] thumbImg;
                if (thumbnailWidth.HasValue & thumbnailHeight.HasValue)
                {
                    // uncomment this if you want to create thumbnail with resizing and filling the blanks with white background
                    //thumbImg = CreateThumbnail(imageFolderPath + "//" + originalFileName, thumbnailWidth.Value, thumbnailHeight.Value)
                    //thumbImg = CreateThumbnailByCropping(imageFolderPath + "//" + originalFileName, thumbnailWidth.Value, thumbnailHeight.Value);
                    thumbImg = imageHelper.CreateThumbnailWithVariableHeight(originalFileNamePath, imageFormat, thumbnailWidth);
                }
                else if (thumbnailWidth.HasValue & !thumbnailHeight.HasValue)
                {
                    thumbImg = imageHelper.CreateThumbnailWithVariableHeight(originalFileNamePath, imageFormat, thumbnailWidth);
                }
                else if (!thumbnailWidth.HasValue & thumbnailHeight.HasValue)
                {
                    thumbImg = imageHelper.CreateThumbnailWithVariableWidth(originalFileNamePath, imageFormat, thumbnailHeight);
                }
                else
                {
                    throw new Exception("No width or height are defined");
                }

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
                    byte[] thumbImg;
                    if (thumbnailWidth.HasValue & thumbnailHeight.HasValue)
                    {
                        // uncomment this if you want to create thumbnail with resizing and filling the blanks with white background
                        //thumbImg = CreateThumbnail(imageFolderPath + "//" + originalFileName, thumbnailWidth.Value, thumbnailHeight.Value)
                        //thumbImg = CreateThumbnailByCropping(imageFolderPath + "//" + originalFileName, thumbnailWidth.Value, thumbnailHeight.Value);
                        thumbImg = imageHelper.CreateThumbnailWithVariableHeight(originalFileNamePath, imageFormat, thumbnailWidth);
                    }
                    else if (thumbnailWidth.HasValue & !thumbnailHeight.HasValue)
                    {
                        thumbImg = imageHelper.CreateThumbnailWithVariableHeight(originalFileNamePath, imageFormat, thumbnailWidth);
                    }
                    else if (!thumbnailWidth.HasValue & thumbnailHeight.HasValue)
                    {
                        thumbImg = imageHelper.CreateThumbnailWithVariableWidth(originalFileNamePath, imageFormat, thumbnailHeight);
                    }
                    else
                    {
                        throw new Exception("No width or height are defined");
                    }

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

    #region "        Format For URL"
    private static string FormatForUrl(string text)
    {
        //log.Debug("Utility function  that formats a string to be accepted for an url.");
        text = text.Replace("%", "%25");
        text = text.Replace("\"", "%22");
        text = text.Replace("#", "%23");
        text = text.Replace("&", "%26");
        text = text.Replace("'", "%27");
        return text;
    }
    #endregion
}
