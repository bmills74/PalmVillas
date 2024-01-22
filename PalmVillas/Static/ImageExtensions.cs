using NuGet.Protocol;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing;
using Azure.Core;

namespace PalmVillas.Static
{
    public static class ImageExtensions
    {
        public static void UploadImages(IFormFile image, Microsoft.AspNetCore.Hosting.IHostingEnvironment _environment)
        {
            var stream = image.OpenReadStream();
            var resized = ImageExtensions.ResizeImage(stream);
            var file = Path.Combine(_environment.ContentRootPath, "wwwroot\\images", image.FileName);
            using (var fileStream = new FileStream(file, FileMode.Create))
            {
                resized.CopyTo(fileStream);
            }
        }
        public static Stream ResizeImage(Stream file)
        {
            if (file.Length == 0)
                return null;          
                using (var srcImage = Bitmap.FromStream(file))
                {
                    var width = 900;
                    int w;
                    float h;
                    if (srcImage.Width > width)
                    {
                        float ratio = (float)srcImage.Width / (float)srcImage.Height;

                        w = width;
                        h = w / ratio;
                    }
                    else
                    {
                        w = srcImage.Width;
                        h = (float)srcImage.Height;
                    }
                    using (Bitmap newImage = new Bitmap((int)w, (int)h))
                    {
                        using (Graphics gr = Graphics.FromImage(newImage))
                        {
                            gr.SmoothingMode = SmoothingMode.HighQuality;
                            gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            gr.PixelOffsetMode = PixelOffsetMode.HighQuality;
                            gr.DrawImage(srcImage, new Rectangle(0, 0, w, (int)h));
                        }
                        var stream = new System.IO.MemoryStream();
                        newImage.Save(stream, ImageFormat.Jpeg);
                        stream.Position = 0;
                        return stream;
                    }
                }


        }
    }
}
