using System.Collections.Generic;
using System.Linq;
using Yomitan.Core.Models;

namespace Yomitan.Models
{
    public class DefinitionModel
    {
        public IEnumerable<TagModel> PartsOfSpeech { get; set; }
        public TagModel Source { get; set; }
        public IEnumerable<string> Definitions { get; set; }

        public DefinitionModel(DefinitionEntry entry)
        {
            PartsOfSpeech = GetTags(entry.PartsOfSpeech);
            Definitions = GetDefinitions(entry.Definitions);
            Source = new TagModel(entry.Source.Name, entry.Source.Category);
        }

        private IEnumerable<TagModel> GetTags(IEnumerable<Tag> tags)
        {
            return tags.Select(t => new TagModel(t.Name, t.Category));
        }

        private IEnumerable<string> GetDefinitions(IEnumerable<Definition> definitions)
        {
            return definitions.Select(d => d.Text);
        }
    }
}
