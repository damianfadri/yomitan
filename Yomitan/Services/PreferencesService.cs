using Newtonsoft.Json;
using System.Threading.Tasks;
using Yomitan.Constants;
using Yomitan.Contracts;
using Yomitan.Core.Helpers;
using Yomitan.Models;
using Yomitan.Strategies;

namespace Yomitan.Services
{
    public class PreferencesService : BaseService, IPreferencesService
    {
        public UserPreferences User { get; private set; }

        public PreferencesService()
        {
            User = new UserPreferences();
        }

        public async override Task InitializeAsync()
        {
            await LoadAsync();

            await base.InitializeAsync();
        }

        public async Task LoadAsync()
        {
            if (!await FileHelper.Exists(YomitanFilepaths.UserPreferencesFilePath))
                await SaveAsync();

            string serialized = await FileHelper.ReadAllTextAsync(YomitanFilepaths.UserPreferencesFilePath);
            User = JsonConvert.DeserializeObject<UserPreferences>(serialized, new UserPreferencesJsonConverter());
        }

        public async Task SaveAsync()
        {
            string serialized = JsonConvert.SerializeObject(User, new UserPreferencesJsonConverter());
            await FileHelper.WriteAllTextAsync(YomitanFilepaths.UserPreferencesFilePath, serialized);
        }
    }
}
