using System.Collections.Generic;
using System.Linq;
using Yomitan.Core.Models;

namespace Yomitan.ViewModel
{
    public class DefinitionViewModel
    {
        public IEnumerable<TagViewModel> PartsOfSpeech { get; set; }
        public TagViewModel Source { get; set; }
        public IEnumerable<string> Definitions { get; set; }

        public DefinitionViewModel(DefinitionEntry entry)
        {
            PartsOfSpeech = GetTags(entry.PartsOfSpeech);
            Definitions = GetDefinitions(entry.Definitions);
            Source = new TagViewModel(entry.Source.Name, entry.Source.Category);
        }

        private IEnumerable<TagViewModel> GetTags(IEnumerable<Tag> tags)
        {
            return tags.Select(t => new TagViewModel(t.Name, t.Category));
        }

        private IEnumerable<string> GetDefinitions(IEnumerable<Definition> definitions)
        {
            return definitions.Select(d => d.Text);
        }
    }
}
