using LifeCalculator.Framework.Services.DataService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeCalculator.Framework.Services.UserService
{
    public interface IUserDataService : IDataService<Users.User>
    {
        #region Methods

        Task<Users.User> LoadByUsername(string username);
        Task<Users.User> LoadByEmail(string email);
        Task<bool> DoesUserExistByEmail(string email);
        Task<bool> DoesUserExistByUsername(string username);

        #endregion
    }
}
