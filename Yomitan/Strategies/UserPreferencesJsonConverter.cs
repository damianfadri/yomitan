using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using Yomitan.Models;

namespace Yomitan.Strategies
{
    internal class UserPreferencesJsonConverter : JsonConverter
    {
        public override bool CanWrite => true;

        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(UserPreferences));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var obj = JObject.Load(reader);

            var termBanks = obj["termBanks"] as JArray;
            var termBankDictionary = new Dictionary<string, TermBankModel>();

            foreach (var termBank in termBanks)
            {
                var name = termBank["name"].ToString();
                var revision = termBank["revision"].ToString();
                var enabled = termBank["enabled"].Value<bool>();

                termBankDictionary[name] = new TermBankModel(name, revision, enabled);
            }

            return new UserPreferences()
            {
                TermBanks = termBankDictionary,
            };
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (!(value is UserPreferences preferences))
                return;

            var main = new JObject();
            var termBanks = new JArray();

            if (preferences.TermBanks != null)
            {
                foreach (var termBankPref in preferences.TermBanks.Values)
                {
                    var termBank = new JObject();
                    termBank["name"] = termBankPref.Title;
                    termBank["revision"] = termBankPref.Revision;
                    termBank["enabled"] = termBankPref.Enabled;

                    termBanks.Add(termBank);
                }
            }

            main["termBanks"] = termBanks;
            main.WriteTo(writer);
        }
    }
}
