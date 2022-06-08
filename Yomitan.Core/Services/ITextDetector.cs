using System.Drawing;
using Yomitan.Core.Models.OCR;

namespace Yomitan.Core.Services
{
    public interface ITextDetector
    {
        Rectangle GetRegion(ImageSource imageSource, Point point);
    }
}
