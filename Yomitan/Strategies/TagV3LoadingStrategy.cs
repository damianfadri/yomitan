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
    internal class TagV3LoadingStrategy : ILoadingStrategy<IEnumerable<Tag>>
    {
        public async Task<IEnumerable<Tag>> ExecuteAsync(string source)
        {
            try
            {
                var content = await FileHelper.ReadAllTextAsync(source);
                var entries = JsonConvert.DeserializeObject<IEnumerable<Tag>>(
                        content, new TagV3JsonConverter());

                return entries.Where(tag => tag != null);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

    internal class TagV3JsonConverter : JsonConverter
    {
        public override bool CanWrite => false;

        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(Tag));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var array = JArray.Load(reader);
            if (array.Count != 5)
                throw new ArgumentException("Invalid format for tag bank.");

            var name = array[0].Value<string>();
            var category = array[1].Value<string>();
            var sortingOrder = array[2].Value<int>();
            var notes = array[3].Value<string>();
            var popularityScore = array[4].Value<int>();

            return new Tag(name, category, sortingOrder, notes, popularityScore);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
