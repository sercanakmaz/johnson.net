using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;

namespace JohnsonNet.Serialization
{
    public class JsonSerializer : ISerializer
    {
        JsonSerializerSettings settings = null;

        private bool p_ToLowerCase;
        public bool ToLowerCase
        {
            get
            {
                return p_ToLowerCase;
            }
            set
            {
                if (value) settings.ContractResolver = new LowercaseContractResolver();
                else settings.ContractResolver = null;

                p_ToLowerCase = value;
            }
        }

        private bool p_ConvertDateToUnixTimeStamp;
        public bool ConvertDateToUnixTimeStamp
        {
            get
            {
                return p_ConvertDateToUnixTimeStamp;
            }
            set
            {
                if (value) settings.Converters.Add(new UnixTimeStampConverter());

                p_ConvertDateToUnixTimeStamp = value;
            }
        }

        private bool p_ConvertEnumToString;
        public bool ConvertEnumToString
        {
            get
            {
                return p_ConvertEnumToString;
            }
            set
            {
                if (value) settings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());

                p_ConvertEnumToString = value;
            }
        }

        public JsonSerializer()
        {
            settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            settings.Converters.Add(new TimeSpanConverter());
        }

        public string Serialize(object input)
        {
            // TODO: Exception objesinin serialize edilmiş hali kontrol edilmeli

            IDataReader reader = input as IDataReader;
            IEnumerable<System.Data.Common.DbDataRecord> records = input as IEnumerable<System.Data.Common.DbDataRecord>;
            NameValueCollection namedCollection = input as NameValueCollection;

            if (reader == null && records == null && namedCollection == null)
                return JsonConvert.SerializeObject(input, settings);

            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();

            if (reader != null)
            {
                while (reader.Read())
                {
                    Dictionary<string, object> row = new Dictionary<string, object>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        row.Add(reader.GetName(i), reader[i]);
                    }
                    rows.Add(row);
                }
            }
            if (records != null)
            {
                foreach (var record in records)
                {
                    Dictionary<string, object> row = new Dictionary<string, object>();
                    for (int i = 0; i < record.FieldCount; i++)
                    {
                        row.Add(record.GetName(i), record[i]);
                    }
                    rows.Add(row);
                }
            }
            if (namedCollection != null)
            {
                foreach (var item in namedCollection.AllKeys)
                {
                    if (string.IsNullOrEmpty(item)) continue;
                    rows.Add(new Dictionary<string, object> { { item, namedCollection[item] } });
                }
            }
            return JsonConvert.SerializeObject(rows, settings);
        }

        public object Deserialize(string input)
        {
            return JsonConvert.DeserializeObject(input, settings);
        }

        public object Deserialize(string input, Type type)
        {
            return JsonConvert.DeserializeObject(input, type, settings);
        }

        public T Deserialize<T>(string input)
        {
            return JsonConvert.DeserializeObject<T>(input, settings);
        }
    }
}
