using System.Collections.Generic;
using Yomitan.Core.Models.Deinflects;
using Yomitan.Core.Models.Terms;

namespace Yomitan.Core.Services
{
    public interface ITermSearchService
    {
        void Add(string key, IEnumerable<Term> terms);

        IEnumerable<Term> Search(string text);

        IEnumerable<Deinflection> Deinflect(string text);
    }
}
