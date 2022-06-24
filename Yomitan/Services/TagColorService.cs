using System.Linq;
using System.Threading.Tasks;
using Yomitan.Constants;
using Yomitan.Contracts;
using Yomitan.Core.Contracts;
using Yomitan.Models;
using Yomitan.Repositories;

namespace Yomitan.Services
{
    public class TagColorService : BaseService, ITagColorService
    {
        private IRepository<TagColor> _colors;

        public TagColorService()
        {
            _colors = new TagColorRepository();
        }

        public TagColor GetTagColor(string category)
        {
            var query = _colors.FindByAsync(category);
            return query.Result.FirstOrDefault();
        }

        public TagColor GetDefaultTagColor()
        {
            var query = _colors.FindAllAsync();
            return query.Result.FirstOrDefault();
        }

        public async override Task InitializeAsync()
        {
            await _colors.LoadAsync(YomitanFilepaths.TagColorsFilePath);
        }
    }
}
