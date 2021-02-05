using Dapper;
using LifeCalculator.Framework.Account;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;

namespace LifeCalculator.Framework.Database.Queries
{
    public class CompoundQueries : BaseQuery
    {
        #region Select Queries

        public static List<CompoundAccount> LoadAll()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<CompoundAccount>("Select * from CompoundAccounts", new DynamicParameters());
                return output.ToList();
            }

            return new List<CompoundAccount>();
        }

        //Change or Delete this
        public static List<CompoundAccount> LoadByName(string Name)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<CompoundAccount>("Select * from CompoundAccounts Where Name = @name", new { name = Name });
                return output.ToList();
            }

            return new List<CompoundAccount>();
        }

        public static CompoundAccount Load(CompoundAccount account)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<CompoundAccount>("Select * from CompoundAccounts Where id = @id", account);
                return output.ToList()[0];
            }

            return new CompoundAccount();
        }

        #endregion

        #region Insert Queries

        public static void Save(CompoundAccount account)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("Insert into CompoundAccounts" +
                    "(Name,InitialAmount,FinalAmount) " +
                    "values (@Name,@InitialAmount,@FinalAmount)", account);
            }
        }

        #endregion

        #region Update Queries

        public static void Update(CompoundAccount account)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("Update CompoundAccounts " +
                    "Set Name=@Name,InitialAmount=@InitialAmount,FinalAmount=@FinalAmount " +
                    "Where ID = @id",
                    account);
            }
        }

        #endregion

        #region Delete Queries

        public static void Delete(CompoundAccount account)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("Delete From CompoundAccounts Where @id", new { id = account.id });
            }
        }

        #endregion
    }
}
