using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Yomitan.Core.Contracts;
using Yomitan.Core.Models;
using Yomitan.Strategies;

namespace Yomitan.Repositories
{
    public class RuleRepository : IRepository<Rule>
    {
        private readonly ICollection<Rule> _rules;

        public RuleRepository()
        {
            _rules = new List<Rule>();
        }

        public async Task<IEnumerable<Rule>> FindAllAsync()
        {
            await Task.CompletedTask;
            return _rules;
        }

        public async Task<IEnumerable<Rule>> FindByAsync(string keyword)
        {
            return (await FindAllAsync()).Where(rule => rule.Name.Equals(keyword));
        }

        public async Task LoadAsync(string filepath)
        {
            if (!File.Exists(filepath))
                throw new FileNotFoundException("Deinflect ruleset is not found.");

            IEnumerable<Rule> rules = await GetLoadingStrategy().ExecuteAsync(filepath);
            if (rules == null)
                throw new InvalidDataException("Deinflect ruleset is not in the correct format.");

            foreach (Rule rule in rules)
                _rules.Add(rule);
        }

        public ILoadingStrategy<IEnumerable<Rule>> GetLoadingStrategy()
        {
            return new RuleLoadingStrategy();
        }
    }
}
