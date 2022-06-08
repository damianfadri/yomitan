using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Yomitan.Core.Models.Terms;
using Yomitan.Core.Services;

namespace Yomitan.Services.Extractors
{
    internal class TagV3Extractor : IExtractor<Tag>
    {
        public bool TryExtract(string source, out IEnumerable<Tag> entries)
        {
            try
            {
                string content = File.ReadAllText(source);
                entries = JsonConvert.DeserializeObject<IEnumerable<Tag>>(
                        content, new TagV3JsonConverter()).Where(tag => tag != null);
            }
            catch (Exception)
            {
                entries = null;
                return false;
            }

            return true;
        }
    }

    internal class TagV3JsonConverter : JsonConverter
    {
        public override bool CanWrite
        {
            get { return false; }
        }

        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(Tag));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JArray array = JArray.Load(reader);
            if (array.Count != 5)
                throw new ArgumentException("Invalid format for tag bank.");

            string name = array[0].Value<string>();
            string category = array[1].Value<string>();
            int sortingOrder = array[2].Value<int>();
            string notes = array[3].Value<string>();
            int popularityScore = array[4].Value<int>();

            return new Tag(name, category, sortingOrder, notes, popularityScore);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
