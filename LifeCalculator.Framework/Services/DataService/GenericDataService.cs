using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LifeCalculator.Framework.Services.DataService
{
    public class GenericDataService<T> : IDataService<T>
    {
        #region Fields

        private string _tableName;

        #endregion

        #region Constructors

        public GenericDataService(string tableName)
        {
            _tableName = tableName;
        }

        #endregion

        #region IDataService Implementation

        public virtual async Task<T> Insert(T entity)
        {
            var saveQuery = GenerateSaveQuery(true);

            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                return await Load( await cnn.ExecuteScalarAsync<int>(saveQuery, entity));
            }
        }

        public virtual async Task Delete(int id)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                await cnn.ExecuteAsync($"DELETE FROM {_tableName} WHERE Id=@Id", new { Id = id });
            }
        }

        public virtual async Task<T> Load(int id)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var result = await cnn.QuerySingleOrDefaultAsync<T>($"SELECT * FROM {_tableName} WHERE Id=@Id", new { Id = id });
                if (result == null)
                    throw new KeyNotFoundException($"{_tableName} with id [{id}] could not be found.");

                return result;
            }
        }

        public async Task<IEnumerable<T>> LoadAll()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = await cnn.QueryAsync<T>($"SELECT * FROM {_tableName}", new DynamicParameters());
                
                return output.AsList<T>();
            }
        }

        public virtual async Task Save(int id, T entity)
        {
            var updateQuery = GenerateUpdateQuery();

            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                await cnn.ExecuteAsync(updateQuery, entity);
            }
        }

        #endregion

        #region QueryGeneratorMethods

        private string GenerateUpdateQuery()
        {
            var updateQuery = new StringBuilder($"UPDATE {_tableName} SET ");
            var properties = GenerateListOfProperties(GetProperties,false);

            properties.ForEach(property =>
            {
                if (!property.Equals("Id"))
                {
                    updateQuery.Append($"{property}=@{property},");
                }
            });

            updateQuery.Remove(updateQuery.Length - 1, 1); //remove last comma
            updateQuery.Append(" WHERE Id=@Id");

            return updateQuery.ToString();
        }

        private string GenerateSaveQuery(bool ignoreID)
        {
            var insertQuery = new StringBuilder($"INSERT INTO {_tableName} ");

            insertQuery.Append("(");

            var properties = GenerateListOfProperties(GetProperties, ignoreID);
            properties.ForEach(prop => { insertQuery.Append($"[{prop}],"); });

            insertQuery
                .Remove(insertQuery.Length - 1, 1)
                .Append(") VALUES (");

            properties.ForEach(prop => { insertQuery.Append($"@{prop},"); });

            insertQuery
                .Remove(insertQuery.Length - 1, 1)
                .Append("); select last_insert_rowid(); ");

            return insertQuery.ToString();
        }

        #endregion

        #region Helper Methods

        private static List<string> GenerateListOfProperties(IEnumerable<PropertyInfo> listOfProperties, bool ignoreID)
        {
            return (from prop in listOfProperties
                    let attributes = prop.GetCustomAttributes(typeof(IgnoreDatabase), false)
                    where attributes.Length <= 0 && !(ignoreID && prop.Name.ToLower().Equals("id"))
                    select prop.Name).ToList();
        }

        private IEnumerable<PropertyInfo> GetProperties => typeof(T).GetProperties();

        public static string LoadConnectionString(string id = "Default")
        {
            return "Data Source=|DataDirectory|\\LifeCalculatorDB.db;Version=3;";
        }

        #endregion
    }
}
