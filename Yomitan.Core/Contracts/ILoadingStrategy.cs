using System.Threading.Tasks;

namespace Yomitan.Core.Contracts
{
    public interface ILoadingStrategy<T>
    {
        Task<T> ExecuteAsync(string source);
    }
}
