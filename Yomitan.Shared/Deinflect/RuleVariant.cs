using System;

namespace Yomitan.Shared.Deinflect
{ 
    public class RuleVariant
    {
        public string KanaIn { get; set; }
        public string KanaOut { get; set; }
        public RuleType RulesIn { get; set; }
        public RuleType RulesOut { get; set; }
    }
}
