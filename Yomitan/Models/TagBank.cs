using System.Collections.Generic;
using System.Linq;
using Yomitan.Core.Models.Terms;
using Yomitan.Core.Services;
using Yomitan.Helpers;

namespace Yomitan.Services.Extractors
{
    internal class TagBank : IBank<Tag>
    {
        private static readonly string _tagInfoPattern = @"tag_bank_*.json";

        private IDictionary<string, Tag> _tags;

        public TagBank(string directoryPath)
        {
            Load(directoryPath);
        }

        private IExtractor<Tag>[] GetTagExtractors()
        {
            return new IExtractor<Tag>[]
            {
                new TagV3Extractor(),
            };
        }

        public void Load(string filepath)
        {
            IDictionary<string, Tag> tags = new Dictionary<string, Tag>();
            foreach (string tagsPath in FileHelper.GetFiles(filepath, _tagInfoPattern))
            {
                IEnumerable<Tag> currTags = null;
                IExtractor<Tag> foundExtractor = GetTagExtractors()
                        .FirstOrDefault(extractor => extractor.TryExtract(tagsPath, out currTags));

                if (foundExtractor == null)
                    continue;

                foreach (Tag tag in currTags)
                    tags[tag.Name] = tag;
            }

            _tags = tags;
        }

        public IEnumerable<Tag> FindAll()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<Tag> FindBy(string keyword)
        {
            return _tags.Values.Where(tag => tag.Name.Equals(keyword));
        }
    }
}
