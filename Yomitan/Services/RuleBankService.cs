using System.Collections.Generic;
using System.Threading.Tasks;
using Yomitan.Constants;
using Yomitan.Core.Contracts;
using Yomitan.Core.Models;
using Yomitan.Repositories;

namespace Yomitan.Services
{
    public class RuleBankService : BaseService, IRuleBankService
    {
        private readonly RuleRepository _rules;

        public RuleBankService()
        {
            _rules = new RuleRepository();
        }

        public async Task<IEnumerable<Rule>> GetAllAsync()
        {
            return await _rules.FindAllAsync();
        }

        public async override Task InitializeAsync()
        {
            await Task.Run(async () =>
            {
                string rulesFilepath = YomitanFilepaths.DeinflectRulesFilePath;
                await _rules.LoadAsync(rulesFilepath);
            });

            await base.InitializeAsync();
        }
    }
}
