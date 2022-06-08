using System.Collections.Generic;

namespace Yomitan.Core.Services
{
    public interface IExtractor<T>
    {
        bool TryExtract(string source, out IEnumerable<T> entries);
    }
}
