namespace Yomitan.Core.Models
{
    public class Deinflection
    {
        public string Text { get; set; }
        public RuleType Rules { get; set; }
        public string[] Reasons { get; set; }

        public Deinflection(string term)
        {
            Text = term;
            Rules = RuleType.None;
            Reasons = null;
        }

        public Deinflection(string term, RuleType ruleType, string[] reasons)
        {
            Text = term;
            Rules = ruleType;
            Reasons = reasons;
        }
    }
}
