using System.Threading.Tasks;

namespace Yomitan.Core.Contracts
{
    public interface IService
    {
        bool Loaded { get; }

        Task InitializeAsync();
    }
}
