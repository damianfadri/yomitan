using System.Threading.Tasks;
using Yomitan.Core.Contracts;
using Yomitan.Models;

namespace Yomitan.Contracts
{
    public interface IPreferencesService : IService
    {
        UserPreferences User { get; }
        Task LoadAsync();
        Task SaveAsync();
    }
}
