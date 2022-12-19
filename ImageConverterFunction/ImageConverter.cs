using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace LambdaTest;

internal class ImageConverter
{
    private readonly Image<Rgba32> _image;

    public ImageConverter(Stream stream)
    {
        _image = Image.Load<Rgba32>(stream);
    }

    public Stream ToGif()
    {
        var ms = new MemoryStream();
        _image.SaveAsGif(ms);
        ms.Position = 0;

        return ms;
    }

    public Stream ToPng()
    {
        var ms = new MemoryStream();
        _image.SaveAsPng(ms);
        ms.Position = 0;

        return ms;
    }

    public Stream ToBmp()
    {
        var ms = new MemoryStream();
        _image.SaveAsBmp(ms);
        ms.Position = 0;

        return ms;
    }
}