using System.Collections.Generic;
using System.Drawing;
using Yomitan.Core.Models.Terms;

namespace Yomitan.Core.Services
{
    public interface ITermDisplayService
    {
        void Display(IEnumerable<Term> terms, Rectangle bounds);
    }
}
