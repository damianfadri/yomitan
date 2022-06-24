using System.Threading.Tasks;

namespace Yomitan.Core.Contracts
{
    public interface ILoadable<T>
    {
        Task LoadAsync(string filepath);

        ILoadingStrategy<T> GetLoadingStrategy();
    }
}
