using Dapper;
using LifeCalculator.Framework.Services.DataService;
using LifeCalculator.Framework.Users;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Threading.Tasks;

namespace LifeCalculator.Framework.Services.UserService
{
    public class UserDataService : GenericDataService<Users.User>, IUserDataService
    {
        #region Fields

        private string _tableName;

        #endregion

        #region Constructors

        public UserDataService(string tableName) : base(tableName)
        {
            _tableName = tableName;
        }

        #endregion

        #region Methods

        public async Task<User> LoadByUsername(string username)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var result = await cnn.QuerySingleOrDefaultAsync<User>($"SELECT * FROM {_tableName} WHERE Username=@Username", new { Username = username });
                if (result == null)
                    throw new KeyNotFoundException($"{_tableName} with user [{username}] could not be found.");

                return result;
            }
        }

        #endregion
    }
}
