namespace Yomitan.Core.Models.Deinflects
{
    public class RuleVariant
    {
        public string KanaIn { get; set; }
        public string KanaOut { get; set; }
        public RuleType RulesIn { get; set; }
        public RuleType RulesOut { get; set; }
    }
}
