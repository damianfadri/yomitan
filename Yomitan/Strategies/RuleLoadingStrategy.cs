using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yomitan.Core.Contracts;
using Yomitan.Core.Helpers;
using Yomitan.Core.Models;

namespace Yomitan.Strategies
{
    internal class RuleLoadingStrategy : ILoadingStrategy<IEnumerable<Rule>>
    {
        public async Task<IEnumerable<Rule>> ExecuteAsync(string source)
        {
            try
            {
                var content = await FileHelper.ReadAllTextAsync(source);
                var entries = JsonConvert.DeserializeObject<IList<Rule>>(content, new YomichanRuleJsonConverter());

                return entries.Where(entry => entry != null);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

    internal class YomichanRuleJsonConverter : JsonConverter
    {
        public override bool CanWrite => false;

        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(IList<Rule>));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            IList<Rule> rules = new List<Rule>();
            var obj = JObject.Load(reader);

            foreach (var ruleName in obj.Properties().Select(p => p.Name))
            {
                var rule = new Rule
                {
                    Name = ruleName,
                    Variants = new List<RuleVariant>()
                };

                JArray ruleVariants = obj[ruleName] as JArray;
                foreach (JObject ruleVariant in ruleVariants)
                {
                    RuleVariant variant = new RuleVariant
                    {
                        KanaIn = ruleVariant["kanaIn"].ToString(),
                        KanaOut = ruleVariant["kanaOut"].ToString()
                    };

                    var rulesIn = ((JArray)ruleVariant["rulesIn"]).ToObject<string[]>();
                    var rulesOut = ((JArray)ruleVariant["rulesOut"]).ToObject<string[]>();

                    variant.RulesIn = Parse(rulesIn);
                    variant.RulesOut = Parse(rulesOut);

                    rule.Variants.Add(variant);
                }

                rules.Add(rule);
            }

            return rules;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        private RuleType Parse(string[] ruleTypes)
        {
            var curr = RuleType.None;

            foreach (var ruleType in ruleTypes)
            {
                var sanitized = ruleType.ToLower().Replace("-", "");
                curr |= (RuleType)Enum.Parse(typeof(RuleType), sanitized, true);
            }

            return curr;
        }
    }
}
