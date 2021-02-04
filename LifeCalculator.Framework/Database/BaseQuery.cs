using System.Configuration;

namespace LifeCalculator.Framework.Database
{
    public class BaseQuery
    {
        public static string LoadConnectionString(string id = "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }
    }
}
