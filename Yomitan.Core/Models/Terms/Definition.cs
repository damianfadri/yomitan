using System.Collections.Generic;

namespace Yomitan.Core.Models.Terms
{
    public class Definition
    {
        public IEnumerable<Tag> PartsOfSpeech { get; set; }
        public IEnumerable<DefinitionText> Definitions { get; set; }
        public Tag Source { get; set; }

        public Definition() { }
    }
}
