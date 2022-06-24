using System;
using System.Collections.Generic;
using System.Drawing;
using Yomitan.Core.Models;

namespace Yomitan.Core.Contracts
{
    public interface ITermDisplayService : IDisposable
    {
        void Display(IEnumerable<Term> terms, Rectangle bounds);
    }
}
