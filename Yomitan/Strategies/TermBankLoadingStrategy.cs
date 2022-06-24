using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Yomitan.Core.Contracts;
using Yomitan.Core.Helpers;
using Yomitan.Core.Models;

namespace Yomitan.Strategies
{
    public class TermBankLoadingStrategy : ILoadingStrategy<TermBank>
    {
        public async Task<TermBank> ExecuteAsync(string source)
        {
            try
            {
                var content = await FileHelper.ReadAllTextAsync(source);
                var info = JsonConvert.DeserializeObject<TermBank>(content);
                if (info == null)
                    return null;

                return info;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
