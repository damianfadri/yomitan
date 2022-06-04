using System.Collections.Generic;
using System.Drawing;

namespace Yomitan.Shared.OCR
{
    public interface IDetector
    {
        TextRegion GetRegion(ImageSource imageSource, Point point);
    }
}
