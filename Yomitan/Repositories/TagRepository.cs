using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yomitan.Core.Contracts;
using Yomitan.Core.Helpers;
using Yomitan.Core.Models;
using Yomitan.Strategies;

namespace Yomitan.Repositories
{
    public class TagRepository : IRepository<Tag>
    {
        private static readonly string _tagInfoPattern = @"tag_bank_*.json";

        private readonly ConcurrentDictionary<string, Tag> _tags;

        public TagRepository()
        {
            _tags = new ConcurrentDictionary<string, Tag>();
        }

        public async Task<IEnumerable<Tag>> FindAllAsync()
        {
            await Task.CompletedTask;
            return _tags.Values;
        }

        public async Task<IEnumerable<Tag>> FindByAsync(string keyword)
        {
            return (await FindAllAsync()).Where(tag => tag.Name.Equals(keyword));
        }

        public async Task LoadAsync(string filepath)
        {
            IEnumerable<Task> extractTasks = FileHelper.GetFiles(filepath, _tagInfoPattern)
                .Select(async tagBankPath =>
                {
                    IEnumerable<Tag> currTags = await GetLoadingStrategy().ExecuteAsync(tagBankPath);
                    foreach (Tag currTag in currTags)
                        _tags.AddOrUpdate(currTag.Name, currTag, (key, oldTag) => currTag);
                });

            await Task.WhenAll(extractTasks);
        }

        public ILoadingStrategy<IEnumerable<Tag>> GetLoadingStrategy()
        {
            return new TagV3LoadingStrategy();
        }
    }
}
