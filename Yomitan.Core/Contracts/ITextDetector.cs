using System.Drawing;
using Yomitan.Core.Models;

namespace Yomitan.Core.Contracts
{
    public interface ITextDetector
    {
        Rectangle GetRegion(ImageSource imageSource, Point point);
    }
}
