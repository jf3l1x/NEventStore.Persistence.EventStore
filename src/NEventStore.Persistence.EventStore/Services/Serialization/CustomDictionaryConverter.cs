using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace NEventStore.Persistence.EventStore.Services
{
    public class CustomDictionaryConverter:JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var dict = (Dictionary<string, object>) value;
            writer.WriteStartArray();
            foreach (var kvp in dict)
            {
                serializer.Serialize(writer,kvp);
            }
            writer.WriteEndArray();
        }
        private static void ReadAndAssert(JsonReader reader)
        {
            if (!reader.Read())
                throw new InvalidOperationException("Unexpected end when reading KeyValuePair.");
        }
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var dict = new Dictionary<string, object>();
            ReadAndAssert(reader);
            while (reader.TokenType == JsonToken.StartObject)
            {
                var item = (KeyValuePair<string,object>)serializer.Deserialize(reader, typeof (KeyValuePair<string, object>));
                dict.Add(item.Key,item.Value);
                ReadAndAssert(reader);
            }
            return dict;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof (Dictionary<string, object>);
        }
    }
}
