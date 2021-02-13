using System;
using System.Configuration;

namespace LifeCalculator.Framework.Database
{
    public class DatabaseManager : IDatabaseManager
    {
        #region Events

        public event EventHandler SaveRequested;

        #endregion

    }
}
