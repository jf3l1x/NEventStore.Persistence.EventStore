using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace NEventStore.Persistence.GES.Services
{
    public class CustomKeyValuePairConverter : JsonConverter
    {
        private const string KeyName = "Key";
        private const string ValueName = "Value";
        private const string TypeName = "TypeName";

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var resolver = serializer.ContractResolver as DefaultContractResolver;
            var kvp = (KeyValuePair<string, object>) value;
            writer.WriteStartObject();
            writer.WritePropertyName((resolver != null) ? resolver.GetResolvedPropertyName(KeyName) : KeyName);
            serializer.Serialize(writer, kvp.Key, typeof (string));
            writer.WritePropertyName((resolver != null) ? resolver.GetResolvedPropertyName(TypeName) : TypeName);
            serializer.Serialize(writer, kvp.Value == null ? typeof(object).FullName : kvp.Value.GetType().FullName,
                typeof(string));
            writer.WritePropertyName((resolver != null) ? resolver.GetResolvedPropertyName(ValueName) : ValueName);
            serializer.Serialize(writer, kvp.Value, kvp.Value == null ? typeof (object) : kvp.Value.GetType());
          
            writer.WriteEndObject();
        }

        private static void ReadAndAssert(JsonReader reader)
        {
            if (!reader.Read())
                throw new InvalidOperationException( "Unexpected end when reading KeyValuePair.");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            string key = null;
            object value = null;
            Type type = null;
            ReadAndAssert(reader);

            while (reader.TokenType == JsonToken.PropertyName)
            {
                string propertyName = reader.Value.ToString();
                if (string.Equals(propertyName, KeyName, StringComparison.OrdinalIgnoreCase))
                {
                    ReadAndAssert(reader);
                    key = (string) serializer.Deserialize(reader, typeof (string));
                }
                else if (string.Equals(propertyName, TypeName, StringComparison.OrdinalIgnoreCase))
                {
                    ReadAndAssert(reader);
                    type = Type.GetType((string) serializer.Deserialize(reader, typeof (string)));
                }
                else if (string.Equals(propertyName, ValueName, StringComparison.OrdinalIgnoreCase))
                {
                    ReadAndAssert(reader);
                    value = serializer.Deserialize(reader, type);
                }
                else
                {
                    reader.Skip();
                }

                ReadAndAssert(reader);
            }

            return new KeyValuePair<string, object>(key, value);
        }

        public override bool CanConvert(Type objectType)
        {
            if (objectType == typeof (KeyValuePair<string, object>))
            {
                return true;
            }
            return false;
        }
    }
}