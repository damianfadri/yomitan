using System.Collections.Generic;

namespace Yomitan.Core.Models
{
    public class DefinitionEntry
    {
        public IEnumerable<Tag> PartsOfSpeech { get; set; }
        public IEnumerable<Definition> Definitions { get; set; }
        public Tag Source { get; set; }
    }
}
