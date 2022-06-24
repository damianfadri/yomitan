using System.Drawing;
using System.IO;

namespace Yomitan.Core.Models
{
    public class ImageSource
    {
        public Image Image { get; private set; }

        public ImageSource(string filepath)
        {
            if (!File.Exists(filepath))
                throw new FileNotFoundException();

            Image = new Bitmap(filepath);
        }
        public ImageSource(Image image)
        {
            Image = image;
        }

        public void Dispose()
        {
            if (Image != null)
                Image.Dispose();
        }
    }
}
