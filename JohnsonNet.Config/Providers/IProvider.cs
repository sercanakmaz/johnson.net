
using System.Data;
namespace JohnsonNet.Config
{
    public interface IProvider
    {
        T GetSetting<T>(string key, T def = default(T));
        string GetSetting(string key, string def = null);
        T GetCommunicationObject<T>();
        System.Configuration.ConnectionStringSettings GetConnectionString(string key);
        IDbConnection GetConnection(string key);
    }
}
