using System.Collections.Generic;

namespace Yomitan.Shared.OCR
{
    public interface IRecognizer
    {
        string Read(ImageSource imageSource);
    }
}
