using System.Collections.Generic;
using Yomitan.Core.Models;

namespace Yomitan.Core.Contracts
{
    public interface IDeinflectionService : IService
    {
        IEnumerable<Deinflection> Deinflect(string text);
    }
}
