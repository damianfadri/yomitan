using IronOcr;
using System.Drawing;
using Yomitan.Shared.OCR;

namespace Yomitan.Tesseract.OCR
{
    public class TesseractRecognizer : IRecognizer
    {
        private readonly IronTesseract _instance;
        public TesseractRecognizer()
        {
            _instance = new IronTesseract
            {
                Language = OcrLanguage.JapaneseVerticalBest,
            };
        }

        public string Read(ImageSource imageSource)
        {
            using (Image image = imageSource.Image)
            using (OcrInput input = new OcrInput(image))
            {
                input.TargetDPI = 96;
                OcrResult result = _instance.Read(input);
                return result.Text;
            }
        }
    }
}
