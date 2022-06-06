using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Yomitan.Shared.Repository;

namespace Yomitan.Shared.Deinflect
{
    public class RuleRepository : IRepository<Rule>
    {
        private static readonly IExtractor<Rule>[] _ruleExtractors =
        {
            new YomichanRuleExtractor(),
        };

        private IEnumerable<Rule> _rules;

        public RuleRepository() { }

        public IEnumerable<Rule> FindAll()
        {
            return _rules;
        }

        public IEnumerable<Rule> FindBy(string keyword)
        {
            return FindAll().Where(rule => rule.Name.Equals(keyword));
        }

        public void Load(RepositoryPath source)
        {
            if (!File.Exists(source.Path))
                throw new FileNotFoundException("Deinflect ruleset is not found.");

            IEnumerable<Rule> rules = null;
            IExtractor<Rule> foundExtractor = _ruleExtractors.FirstOrDefault(extractor => extractor.TryExtract(source.Path, out rules));
            if (foundExtractor == null)
                throw new InvalidDataException("Deinflect ruleset is not in the correct format.");

            _rules = rules;
        }

        public void Unload(RepositoryPath source)
        {
            throw new NotImplementedException();
        }
    }
}
