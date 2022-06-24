using System.Threading.Tasks;
using Yomitan.Core.Contracts;

namespace Yomitan.Services
{
    public abstract class BaseService : IService
    {
        public bool Loaded { get; protected set; }

        public async virtual Task InitializeAsync()
        {
            await Task.CompletedTask;
            Loaded = true;
        }
    }
}
