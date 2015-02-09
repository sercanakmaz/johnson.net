using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JohnsonNet.Serialization
{
    public class AbsoluteUriConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Uri);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            string val = Core.ConvertObject<string>(reader.Value);

            if (string.IsNullOrEmpty(val)) return null;

            Uri result = null;
            if (!Uri.TryCreate(val, UriKind.RelativeOrAbsolute, out result))
                return null;

            if (result == null) return null;

            return new Uri(result.PathAndQuery, UriKind.RelativeOrAbsolute);
        }

        public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            Uri result = value as Uri;
            string baseUri = Provider.Config.GetSetting("AbsoluteUriConverter-BaseUri");

            if (result != null && !string.IsNullOrEmpty(baseUri))
            {
                if (Uri.IsWellFormedUriString(result.OriginalString, UriKind.Absolute))
                {
                    result = new Uri(new Uri(baseUri), result.PathAndQuery);
                }
                else
                {
                    result = new Uri(new Uri(baseUri), result.OriginalString);
                }
            }

            writer.WriteValue(result.OriginalString);
        }
    }
}
