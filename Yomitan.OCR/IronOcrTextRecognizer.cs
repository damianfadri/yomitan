using IronOcr;
using System.Drawing;
using Yomitan.Core.Contracts;
using Yomitan.Core.Helpers;
using Yomitan.Core.Models;

namespace Yomitan.OCR
{
    public class IronOcrTextRecognizer : ITextRecognizer
    {
        private readonly IronTesseract _instance;

        public IronOcrTextRecognizer()
        {
            _instance = new IronTesseract
            {
                Language = OcrLanguage.JapaneseVerticalBest,
            };
        }

        public TextRegion Read(ImageSource imageSource)
        {
            using (Image image = imageSource.Image)
            using (OcrInput input = new OcrInput(image))
            {
                input.TargetDPI = 96;
                OcrResult result = _instance.Read(input);
                if (result.Confidence < 80)
                    return null;

                Rectangle bounds = CalculateBounds(result);
                return new TextRegion(bounds, result.Text);
            }
        }

        private Rectangle CalculateBounds(OcrResult result)
        {
            Rectangle bounds = Rectangle.Empty;
            foreach (OcrResult.Block block in result.Blocks)
                bounds = bounds.Append(block.Location);

            return bounds;
        }
    }
}
