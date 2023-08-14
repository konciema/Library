using System.Drawing.Drawing2D;
using System.Drawing;
using System.Data;
using System.Reflection;

namespace Library
{
    public static class AuxiliaryFunctions
    {
        public static string[] ValidImageTypes = new string[] { "image/gif", "image/jpeg", "image/pjpeg", "image/png", "image/bmp" };
        public static byte[] CreateThumbnail(byte[] image, int thumbWidth)
        {
            MemoryStream msImage = new MemoryStream(image);
            System.Drawing.Image fullsizeImage = System.Drawing.Image.FromStream(msImage);

            int thumbHeight = (int)(fullsizeImage.Height * thumbWidth / (double)fullsizeImage.Width);

            var thumbnailBitmap = new Bitmap(thumbWidth, thumbHeight);
            Graphics thumbnailGraph = Graphics.FromImage(thumbnailBitmap);
            thumbnailGraph.CompositingQuality = CompositingQuality.HighQuality;
            thumbnailGraph.SmoothingMode = SmoothingMode.HighQuality;
            thumbnailGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;

            var imageRectangle = new Rectangle(0, 0, thumbWidth, thumbHeight);
            thumbnailGraph.DrawImage(fullsizeImage, imageRectangle);

            MemoryStream msThumb = new MemoryStream();
            thumbnailBitmap.Save(msThumb, fullsizeImage.RawFormat);

            fullsizeImage.Dispose();
            thumbnailGraph.Dispose();

            return msThumb.ToArray();
        }

    }
}
