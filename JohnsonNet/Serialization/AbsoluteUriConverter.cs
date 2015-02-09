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

            return result;
        }

        public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            Uri typedValue = value as Uri;
            if (typedValue == null) return;

            string baseUri = Provider.Config.GetSetting("AbsoluteUriConverter-BaseUri");

            // IsRelativeUri
            if (Uri.IsWellFormedUriString(typedValue.OriginalString, UriKind.Relative) && !string.IsNullOrEmpty(baseUri))
            {
                Uri baseUriTyped = new Uri(baseUri);
                writer.WriteValue(new Uri(baseUriTyped, typedValue.OriginalString));
                return;
            }

            writer.WriteValue(typedValue.OriginalString);
        }
    }
}
