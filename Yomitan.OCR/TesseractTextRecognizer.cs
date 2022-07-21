using System.Drawing;
using Tesseract;
using Yomitan.Core.Contracts;
using Yomitan.Core.Models;

namespace Yomitan.OCR
{
    public class TesseractTextRecognizer : ITextRecognizer
    {
        public TextRegion Read(ImageSource source)
        {
            using (var engine = new TesseractEngine("./tessdata", "jpn_vert"))
            using (var page = engine.Process(new Bitmap(source.Image)))
            {
                var region = new TextRegion()
                {
                    Text = page.GetText(),
                };

                return region;
            }
        }
    }
}
