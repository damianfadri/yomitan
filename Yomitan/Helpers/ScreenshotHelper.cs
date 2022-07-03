using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace Yomitan.Core.Helpers
{
    public static class ScreenshotHelper
    {
        public static Bitmap AllScreens()
        {
            return Bounds(SystemInformation.VirtualScreen);
        }

        public static Bitmap CurrentScreen()
        {
            Point currentPos = Cursor.Position;
            Screen screen = Screen.FromPoint(currentPos);
            return Bounds(screen.Bounds);
        }

        public static Bitmap Bounds(Rectangle bounds)
        {
            Bitmap bmp = new Bitmap(bounds.Width, bounds.Height);
            bmp.SetResolution(225, 225);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.CopyFromScreen(bounds.Left, bounds.Top, 0, 0, bmp.Size);
            }

            return bmp;
        }

        public static Bitmap Bounds(Image source, Rectangle bounds)
        {
            Bitmap bmp = new Bitmap(bounds.Width, bounds.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.DrawImage(source, new Rectangle(0, 0, bmp.Width, bmp.Height), bounds, GraphicsUnit.Pixel);
            }

            return bmp;
        }

        public static void ToFile(Image image, string filepath)
        {
            ToFile(image, filepath, ImageFormat.Png);
        }

        public static void ToFile(Image image, string filepath, ImageFormat format)
        {
            string directoryPath = Path.GetDirectoryName(filepath);
            Directory.CreateDirectory(directoryPath);

            image.Save(filepath, format);
        }
    }
}
