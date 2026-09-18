using dotnet_image_thumbnail.Library.Image;
using dotnet_image_thumbnail.Library.Image.Skia;

namespace dotnet_image_thumbnail.Tests;

public class ImageThumbnailManagerTests
{
    private static ImageThumbnailManager CreateManager() =>
        new(new SkiaImageQuantizer(), new SkiaImageDecoder(), new SkiaImageHelper());

    [Fact]
    public void RetrieveThumbnailFileName_CreatesThumbnailFile()
    {
        string source = TestImages.CreateTempPngFile(200, 100);
        try
        {
            string name = CreateManager().RetrieveThumbnailFileName(source, 80, null);

            Assert.StartsWith("Thumbnail_", name);
            Assert.EndsWith(".png", name);

            string thumbnailPath = Path.Combine(Path.GetDirectoryName(source)!, name);
            Assert.True(File.Exists(thumbnailPath));

            File.Delete(thumbnailPath);
        }
        finally
        {
            File.Delete(source);
        }
    }

    [Fact]
    public void RetrieveThumbnailFileName_NoDimensions_Throws()
    {
        string source = TestImages.CreateTempPngFile(50, 50);
        try
        {
            Assert.Throws<Exception>(() => CreateManager().RetrieveThumbnailFileName(source, null, null));
        }
        finally
        {
            File.Delete(source);
        }
    }
}
