using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JohnsonNet.Serialization
{
    public class UnixTimeStampConverter : DateTimeConverterBase
    {
        public bool IsSecond { get; set; }
        public UnixTimeStampConverter() { }
        public UnixTimeStampConverter(bool isSecond)
        {
            this.IsSecond = IsSecond;
        }
        public static readonly DateTime UnixOrigin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            var converted = JohnsonManager.Convert.To<long?>(reader.Value);
            if (!converted.HasValue)
                return null;

            if (IsSecond)
                return UnixOrigin.AddSeconds(converted.Value);
            else
                return UnixOrigin.AddMilliseconds(converted.Value);
        }
        public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            var converted = JohnsonManager.Convert.To<DateTime?>(value);

            if (!converted.HasValue)
                writer.WriteValue(converted);
            else
            {
                var unixTime = (converted.Value - UnixOrigin);

                if (IsSecond)
                    writer.WriteValue((long)unixTime.TotalSeconds);
                else
                    writer.WriteValue((long)unixTime.TotalMilliseconds);
            }
        }
    }
}
