using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using Yomitan.Shared.Repository;

namespace Yomitan.Shared.Deinflect
{
    internal class YomichanRuleExtractor : IExtractor<Rule>
    {
        public bool TryExtract(string source, out IEnumerable<Rule> entries)
        {
            try
            {
                string content = File.ReadAllText(source);
                YomichanRuleCollection container = JsonConvert.DeserializeObject<YomichanRuleCollection>(
                            content, new YomichanRuleJsonConverter());

                entries = container.Rules;
            }
            catch (Exception)
            {
                entries = null;
                return false;
            }

            return true;
        }
    }
}
