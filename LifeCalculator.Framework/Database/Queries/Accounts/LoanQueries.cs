using LifeCalculator.Framework.SimulatedAccount;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SQLite;
using LifeCalculator.Framework.Database;
using Dapper;

namespace LifeCalculator.Framework.Queries
{
    public class LoanQueries : BaseQuery
    {
        #region Select Queries

        public static List<LoanAccount> LoadAll()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<LoanAccount>("Select * from LoanAccounts", new DynamicParameters());
                return output.ToList();
            }

            return new List<LoanAccount>();
        }

        //Change or Delete this
        public static List<LoanAccount> LoadByName(string Name)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<LoanAccount>("Select * from LoanAccounts Where Name = @name", new { name = Name });
                return output.ToList();
            }

            return new List<LoanAccount>();
        }

        public static LoanAccount Load(LoanAccount account)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<LoanAccount>("Select * from LoanAccounts Where id = @id", account);
                return output.ToList()[0];
            }

            return new LoanAccount();
        }

        #endregion

        #region Insert Queries

        public static void Save(LoanAccount account)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("Insert into LoanAccounts" +
                    " (Name,DownPayment,InterestPaid,InterestRate,LoanAmount,LoanLengthMonths,MonthlyPayment,PrincipalPaid) " +
                    "values (@Name,@DownPayment,@InterestPaid,@InterestRate,@LoanAmount,@LoanLengthMonths,@MonthlyPayment,@PrincipalPaid)", account);
            }
        }

        #endregion

        #region Update Queries

        public static void Update(LoanAccount account)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("Update LoanAccounts " +
                    "Set Name=@Name,DownPayment=@DownPayment,InterestPaid=@InterestPaid,InterestRate=@InterestRate,LoanAmount=@LoanAmount," +
                    "LoanLengthMonths=@LoanLengthMonths,MonthlyPayment=@MonthlyPayment,PrincipalPaid=@PrincipalPaid " +
                    "Where ID = @id",
                    account);
            }
        }

        #endregion

        #region Delete Queries

        public static void Delete(LoanAccount account)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("Delete From LoanAccounts Where @id", new { id = account.Id });
            }
        }

        #endregion
    }
}
