using System.Collections.Generic;
using System.IO;
using System.Linq;
using Yomitan.Core.Models.Deinflects;
using Yomitan.Core.Services;
using Yomitan.Services.Extractors;

namespace Yomitan.Models
{
    internal class RuleBank : IBank<Rule>
    {
        private IEnumerable<Rule> _rules;

        public RuleBank()
        {
            _rules = new List<Rule>();
        }

        private IExtractor<Rule>[] GetRuleExtractors()
        {
            return new IExtractor<Rule>[]
            {
                new YomichanRuleExtractor(),
            };
        }

        public void Load(string filepath)
        {
            if (!File.Exists(filepath))
                throw new FileNotFoundException("Deinflect ruleset is not found.");

            IEnumerable<Rule> rules = null;
            IExtractor<Rule> foundExtractor = GetRuleExtractors().FirstOrDefault(extractor => extractor.TryExtract(filepath, out rules));
            if (foundExtractor == null)
                throw new InvalidDataException("Deinflect ruleset is not in the correct format.");

            _rules = rules;
        }

        public IEnumerable<Rule> FindAll()
        {
            return _rules;
        }

        public IEnumerable<Rule> FindBy(string keyword)
        {
            return _rules.Where(rule => rule.Name.Equals(keyword));
        }
    }
}
