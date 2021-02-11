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

        #endregion
    }
}
