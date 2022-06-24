using Yomitan.Core.Models;

namespace Yomitan.Core.Contracts
{
    public interface ITextRecognizer
    {
        TextRegion Read(ImageSource source);
    }
}
