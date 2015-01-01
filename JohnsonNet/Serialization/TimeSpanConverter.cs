using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JohnsonNet.Serialization
{
    public class TimeSpanConverter : JsonConverter
    {
        public TimeSpanConverter() { }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            var converted = Core.ConvertObject<long?>(reader.Value);
            if (!converted.HasValue)
                return null;

            return new TimeSpan(TimeSpan.TicksPerMillisecond * converted.Value);
        }
        public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            var converted = value as TimeSpan?;

            if (!converted.HasValue)
                writer.WriteValue(converted);
            else
            {
                writer.WriteValue((long)converted.Value.TotalMilliseconds);
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(TimeSpan)
                || objectType == typeof(TimeSpan?);
        }
    }
}
