using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yomitan.Shared.Term
{
    public class Definition
    {
        public IEnumerable<Tag> PartsOfSpeech { get; set; }
        public IEnumerable<DefinitionText> Definitions { get; set; }

        public Definition()
        {

        }
    }
}
