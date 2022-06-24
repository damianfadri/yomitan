using System.Collections.Generic;
using System.Threading.Tasks;

namespace Yomitan.Core.Contracts
{
    public interface IRepository<T> : ILoadable<IEnumerable<T>>
    {
        Task<IEnumerable<T>> FindAllAsync();

        Task<IEnumerable<T>> FindByAsync(string keyword);
    }
}
