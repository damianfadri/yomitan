using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Yomitan.Shared.Term
{
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
