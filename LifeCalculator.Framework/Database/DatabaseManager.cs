using System;
using System.Configuration;

namespace LifeCalculator.Framework.Database
{
    public class DatabaseManager : IDatabaseManager
    {
        #region MyRegion

        public event EventHandler SaveRequested;

        #endregion

    }
}
