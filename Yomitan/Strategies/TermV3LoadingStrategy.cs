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
    public class TermV3LoadingStrategy : ILoadingStrategy<IEnumerable<Term>>
    {
        public async Task<IEnumerable<Term>> ExecuteAsync(string source)
        {
            try
            {
                var content = await FileHelper.ReadAllTextAsync(source);
                var entries = JsonConvert.DeserializeObject<IEnumerable<Term>>(content, new TermV3JsonConverter());

                return entries.Where(term => term != null);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

    internal class TermV3JsonConverter : JsonConverter
    {
        public override bool CanWrite => false;

        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(Term));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var array = JArray.Load(reader);
            if (array.Count != 8)
                throw new ArgumentException("Invalid format for term bank.");

            var kanji = array[0].Value<string>();
            var reading = array[1].Value<string>();

            var partsOfSpeech = array[2].Value<string>().Tokenize()
                    .Select(strPartOfSpeech => new Tag(strPartOfSpeech));

            IEnumerable<string> ruleIdentifiers = array[3].Value<string>().Tokenize();
            var popularityScore = array[4].Value<int>();

            var definitions = array[5].ToObject<string[]>()
                    .Select(strDefinition => new Definition(strDefinition));

            var sequenceNum = array[6].Value<int>();

            var tags = array[7].Value<string>().Tokenize()
                    .Select(strTag => new Tag(strTag));

            var definition = new DefinitionEntry()
            {
                Definitions = definitions,
                PartsOfSpeech = partsOfSpeech,
            };

            var definitionEntries = new DefinitionEntry[] { definition };

            var term = new Term()
            {
                Text = kanji,
                Reading = reading,
                Tags = tags,
                DefinitionEntries = definitionEntries,
                SequenceNumber = sequenceNum,
                PopularityScore = popularityScore,
            };

            return term;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
