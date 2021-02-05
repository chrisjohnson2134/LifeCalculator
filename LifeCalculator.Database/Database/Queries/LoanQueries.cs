using LifeCalculator.Framework.Account;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SQLite;
using Dapper;
using LifeCalculator.Database.Database;

namespace LifeCalculator.Database.Queries
{
    public class LoanQueries : DatabaseManager
    {
        public static List<LoanAccount> LoadLoanAccounts()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<LoanAccount>("Select * from LoanAccounts", new DynamicParameters());
                return output.ToList();
            }

            return new List<LoanAccount>();
        }

        public static void SaveLoanAccount(LoanAccount account)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("Insert into LoanAccounts (Name) values (@Name)", account);
            }
        }
    }
}
