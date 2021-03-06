using System.Collections.Generic;
using System.Threading.Tasks;
using Yomitan.Contracts;
using Yomitan.Core.Contracts;

namespace Yomitan.Services
{
    public class InitializerService : BaseService, IInitializerService
    {
        private readonly ICollection<IService> _services;

        public InitializerService(IRequiredFilesService configurationService, ITagColorService tagColorService, IPreferencesService preferencesService)
        {
            _services = new List<IService>()
            {
                configurationService,
                tagColorService,
                preferencesService,
            };
        }

        public async override Task InitializeAsync()
        {
            foreach (var service in _services)
                if (!service.Loaded)
                    await service.InitializeAsync();

            await base.InitializeAsync();
        }
    }
}
