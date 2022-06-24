using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yomitan.Core.Contracts;
using Yomitan.Core.Helpers;
using Yomitan.Models;

namespace Yomitan.Strategies
{
    public class TagColorLoadingStrategy : ILoadingStrategy<IEnumerable<TagColor>>
    {
        public async Task<IEnumerable<TagColor>> ExecuteAsync(string source)
        {
            try
            {
                var content = await FileHelper.ReadAllTextAsync(source);
                var entries = JsonConvert.DeserializeObject<IEnumerable<TagColor>>(content);

                return entries.Where(color => color != null);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
