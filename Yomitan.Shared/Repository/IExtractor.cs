using System.Collections.Generic;

namespace Yomitan.Shared.Repository
{
    public interface IExtractor<T>
    {
        bool TryExtract(string source, out IEnumerable<T> entries);
    }
}
