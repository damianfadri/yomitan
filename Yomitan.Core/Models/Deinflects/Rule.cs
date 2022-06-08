using System.Collections.Generic;

namespace Yomitan.Core.Models.Deinflects
{
    public class Rule
    {
        public string Name { get; set; }
        public IList<RuleVariant> Variants { get; set; }
    }
}
