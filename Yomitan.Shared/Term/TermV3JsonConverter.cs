using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using Yomitan.Shared.Repository;
using Yomitan.Shared.Utils;

namespace Yomitan.Shared.Term
{
    internal class TermV3JsonConverter : JsonConverter
    {
        public override bool CanWrite
        {
            get { return false; }
        }

        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(Term));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JArray array = JArray.Load(reader);
            if (array.Count != 8)
                throw new ArgumentException("Invalid format for term bank.");

            string kanji = array[0].Value<string>();
            string reading = array[1].Value<string>();

            Expression expression = new Expression(kanji, reading);

            IEnumerable<Tag> partsOfSpeech = array[2].Value<string>().Tokenize()
                    .Select(strPartOfSpeech => new Tag(strPartOfSpeech));

            IEnumerable<string> ruleIdentifiers = array[3].Value<string>().Tokenize();
            int popularityScore = array[4].Value<int>();

            IEnumerable<DefinitionText> definitions = array[5].ToObject<string[]>()
                    .Select(strDefinition => new DefinitionText(strDefinition));

            int sequenceNum = array[6].Value<int>();
            IEnumerable<Tag> tags = array[7].Value<string>().Tokenize()
                    .Select(strTag => new Tag(strTag));

            Definition definition = new Definition()
            {
                Definitions = definitions,
                PartsOfSpeech = partsOfSpeech,
            };

            IEnumerable<Definition> definitionList = new List<Definition>() { definition };

            Term term = new Term()
            {
                Expression = expression,
                Tags = tags,
                Definitions = definitionList,
                RuleIdentifiers = ruleIdentifiers,
                SequenceNum = sequenceNum,
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
