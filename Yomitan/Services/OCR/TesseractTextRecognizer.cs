using IronOcr;
using System.Drawing;
using Yomitan.Core.Models.OCR;
using Yomitan.Core.Services;
using Yomitan.Helpers;

namespace Yomitan.Services.OCR
{
    internal class TesseractTextRecognizer : ITextRecognizer
    {
        private IronTesseract _instance;

        public TesseractTextRecognizer()
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
