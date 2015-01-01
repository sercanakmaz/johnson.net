using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JohnsonNet.Serialization
{
    public interface ISerializer
    {
        string Serialize(object input);
        object Deserialize(string input);
        object Deserialize(string input, Type type);
        T Deserialize<T>(string input);
    }
}
