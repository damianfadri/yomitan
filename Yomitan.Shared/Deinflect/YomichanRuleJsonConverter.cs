﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Yomitan.Shared.Deinflect
{
    internal class YomichanRuleJsonConverter : JsonConverter
    {
        public override bool CanWrite
        {
            get { return false; }
        }

        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(YomichanRuleCollection));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            IList<Rule> rules = new List<Rule>();
            JObject obj = JObject.Load(reader);

            foreach (string ruleName in obj.Properties().Select(p => p.Name))
            {
                Rule rule = new Rule
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

                    string[] rulesIn = ((JArray)ruleVariant["rulesIn"]).ToObject<string[]>();
                    string[] rulesOut = ((JArray)ruleVariant["rulesOut"]).ToObject<string[]>();

                    variant.RulesIn = Parse(rulesIn);
                    variant.RulesOut = Parse(rulesOut);

                    rule.Variants.Add(variant);
                }

                rules.Add(rule);
            }

            return new YomichanRuleCollection
            {
                Rules = rules,
            };
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        private RuleType Parse(string[] ruleTypes)
        {
            RuleType curr = RuleType.None;

            foreach (string ruleType in ruleTypes)
            {
                string sanitized = ruleType.ToLower().Replace("-", "");
                curr |= (RuleType)Enum.Parse(typeof(RuleType), sanitized, true);
            }

            return curr;
        }
    }
}
