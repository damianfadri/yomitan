using System.Collections.Generic;

namespace Yomitan.Shared.Deinflect
{
    public class Rule
    {
        public string Name { get; set; }
        public IList<RuleVariant> Variants { get; set; }
    }
}
