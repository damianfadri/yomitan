using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Yomitan.Core.Contracts;
using Yomitan.Models;
using Yomitan.Strategies;

namespace Yomitan.Repositories
{
    internal class TagColorRepository : IRepository<TagColor>
    {
        private readonly ICollection<TagColor> _colors;

        public TagColorRepository()
        {
            _colors = new List<TagColor>();
        }

        public async Task<IEnumerable<TagColor>> FindAllAsync()
        {
            await Task.CompletedTask;
            return _colors;
        }

        public async Task<IEnumerable<TagColor>> FindByAsync(string keyword)
        {
            return (await FindAllAsync()).Where(color => keyword.Equals(color.Category));
        }

        public async Task LoadAsync(string filepath)
        {
            if (!File.Exists(filepath))
                throw new FileNotFoundException("Tag color schema is not found.");

            IEnumerable<TagColor> colors = await GetLoadingStrategy().ExecuteAsync(filepath);
            if (colors == null)
                throw new InvalidDataException("Deinflect ruleset is not in the correct format.");

            foreach (TagColor color in colors)
                _colors.Add(color);
        }

        public ILoadingStrategy<IEnumerable<TagColor>> GetLoadingStrategy()
        {
            return new TagColorLoadingStrategy();
        }
    }
}
