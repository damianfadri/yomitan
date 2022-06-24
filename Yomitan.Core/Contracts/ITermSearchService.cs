using System.Collections.Generic;
using Yomitan.Core.Models;

namespace Yomitan.Core.Contracts
{
    public interface ITermSearchService : IService
    {
        void Index(string key, IEnumerable<Term> terms);

        void Unindex(string key);

        void Enable(string key);

        void Disable(string key);

        IEnumerable<Term> Search(string keyword);
    }
}
