using Accord.Imaging;
using Accord.Imaging.Filters;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace Yomitan.Shared.Utils
{
    public static class BitmapHelper
    {
        public static Bitmap CopyImage(this Bitmap image)
        {
            var newImage = new Bitmap(image);
            return newImage;
        }

        public static Bitmap Sharpen(this Bitmap bitmap)
        {
            GaussianSharpen sharpen = new GaussianSharpen();
            Bitmap result = sharpen.Apply(bitmap);
            return result;
        }

        public static Bitmap Grayscale(this Bitmap bitmap)
        {
            Grayscale filter = new Grayscale(0.2125, 0.7154, 0.0721);
            Bitmap result = filter.Apply(bitmap);
            return result;
        }

        public static Bitmap DetectEdges(this Bitmap bitmap, byte lowThreshold, byte highThreshold, double sigma)
        {
            CannyEdgeDetector filter = new CannyEdgeDetector(lowThreshold, highThreshold, sigma);
            filter.ApplyInPlace(bitmap);
            return bitmap;
        }

        public static Bitmap Binarize(this Bitmap bitmap, int threshold)
        {
            Threshold filter = new Threshold(threshold);
            filter.ApplyInPlace(bitmap);
            return bitmap;
        }

        public static Bitmap Binarize(this Bitmap bitmap)
        {
            Threshold filter = new Threshold();
            filter.ApplyInPlace(bitmap);
            return bitmap;
        }

        public static Bitmap Dilate(this Bitmap bitmap)
        {
            Dilation dilation = new Dilation();
            dilation.ApplyInPlace(bitmap);
            return bitmap;
        }

        public static Bitmap VerticalSmear(this Bitmap bitmap, int smear)
        {
            VerticalRunLengthSmoothing vrls = new VerticalRunLengthSmoothing(smear);
            vrls.ApplyInPlace(bitmap);
            return bitmap;
        }

        public static Bitmap HorizontalSmear(this Bitmap bitmap, int smear)
        {
            HorizontalRunLengthSmoothing vrls = new HorizontalRunLengthSmoothing(smear);
            vrls.ApplyInPlace(bitmap);
            return bitmap;
        }

        public static Bitmap GaussianBlur(this Bitmap bitmap, double sigma)
        {
            GaussianBlur blur = new GaussianBlur(sigma);
            blur.ApplyInPlace(bitmap);
            return bitmap;
        }

        public static Bitmap ToFile(this Bitmap bitmap, string filepath)
        {
            bitmap.Save(filepath, ImageFormat.Png);
            return bitmap;
        }

        public static IEnumerable<Rectangle> GetBlobs(this Bitmap bitmap)
        {
            BlobCounter bc = new BlobCounter();
            bc.ProcessImage(bitmap);

            return bc.GetObjects(bitmap, true).Select(blob => blob.Rectangle);
        }

        public static Bitmap DrawBounds(this Bitmap bitmap, IEnumerable<Rectangle> bounds, Color color)
        {
            Bitmap temp = new Bitmap(bitmap);
            using (Graphics g = Graphics.FromImage(temp))
            {
                using (Pen pen = new Pen(color, 3))
                {
                    foreach (Rectangle bound in bounds)
                    {
                        g.DrawRectangle(pen, bound);
                    }
                }
            }

            return temp;
        }
        
        public static Bitmap DrawBounds(this Bitmap bitmap, IEnumerable<Rectangle> bounds)
        {
            return DrawBounds(bitmap, bounds, Color.Red);
        }

        public static Bitmap Pad(this Bitmap bitmap, Color color)
        {
            int borderSize = 20;
            Bitmap temp = new Bitmap(bitmap.Width + borderSize * 2, bitmap.Height + borderSize * 2);
            using (Graphics g = Graphics.FromImage(temp))
            {
                using (Brush border = new SolidBrush(color))
                {
                    g.FillRectangle(border, 0, 0, bitmap.Width + 2 * borderSize, bitmap.Height + 2 * borderSize);
                }

                g.DrawImage(bitmap, borderSize, borderSize);
            }

            return temp;
        }
    }
}
