using System;
using System.IO;
using Newtonsoft.Json;

namespace NEventStore.Persistence.GES.Services
{
    public class JsonNetSerializer : IEventStoreSerializer
    {
        public bool IsJsonSerializer
        {
            get { return true; }
        }

        public byte[] Serialize(object graph)
        {
            var serializer = new JsonSerializer {NullValueHandling = NullValueHandling.Ignore};

            serializer.Converters.Add(new CustomDictionaryConverter());
            serializer.Converters.Add(new CustomKeyValuePairConverter());
            using (var ms = new MemoryStream())
            {
                using (var sw = new StreamWriter(ms))
                {
                    using (JsonWriter writer = new JsonTextWriter(sw))
                    {
                        serializer.Serialize(writer, graph);
                        writer.Flush();
                        return ms.ToArray();
                    }
                }
            }
        }

        public object Deserialize(string type, byte[] data)
        {
            var serializer = new JsonSerializer {NullValueHandling = NullValueHandling.Ignore};
            serializer.Converters.Add(new CustomDictionaryConverter());
            serializer.Converters.Add(new CustomKeyValuePairConverter());
            using (var ms = new MemoryStream(data))
            {
                using (var sw = new StreamReader(ms))
                {
                    using (var reader = new JsonTextReader(sw))
                    {
                        return serializer.Deserialize(reader, Type.GetType(type));
                    }
                }
            }
        }
    }
}