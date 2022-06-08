using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yomitan.Core.Models.OCR;

namespace Yomitan.Core.Services
{
    public interface ITextRecognizer
    {
        TextRegion Read(ImageSource source);
    }
}
