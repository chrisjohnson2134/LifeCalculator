using Dapper;
using LifeCalculator.Framework.CustomExceptions;
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
                    throw new UserNotFoundException($"username [{username}] could not be found.");

                return result;
            }
        }

        public async Task<User> LoadByEmail(string email)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var result = await cnn.QuerySingleOrDefaultAsync<User>($"SELECT * FROM {_tableName} WHERE Email=@Email", new { Email = email });
                if (result == null)
                    throw new UserNotFoundException($"user with email [{email}] could not be found.");

                return result;
            }
        }

        public async Task<bool> DoesUserExistByEmail(string email)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var userExists = false;
                var result = await cnn.QuerySingleOrDefaultAsync<User>($"SELECT * FROM {_tableName} WHERE Email=@Email", new { Email = email });
                if (result == null)
                    userExists = false;
                else
                    userExists = true;

                return userExists;
            }
        }

        public async Task<bool> DoesUserExistByUsername(string username)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var userExists = false;
                var result = await cnn.QuerySingleOrDefaultAsync<User>($"SELECT * FROM {_tableName} WHERE Username=@Username", new { Username = username });
                if (result == null)
                    userExists = false;
                else
                    userExists = true;

                return userExists;
            }
        }

        #endregion
    }
}
