using System;
using System.Configuration;

namespace LifeCalculator.Database.Database
{
    public class DatabaseManager : IDatabaseManager
    {
        #region MyRegion

        public event EventHandler SaveRequested;

        #endregion

        #region Static Methods

        public static string LoadConnectionString(string id = "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }

        #endregion


    }
}
