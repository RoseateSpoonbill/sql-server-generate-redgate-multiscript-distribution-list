using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;

namespace MSSQL.Extension
{
    public static class DapperExtension
    {
        public static Task<IEnumerable<T>> QueryStoredProcedureAsync<T>(this SqlConnection sqlConnection, string storedProcedureName, object param = null, System.Data.IDbTransaction dbTransaction = null, bool buffered = true, int? commandTimeout = null)
        {
            return sqlConnection.QueryAsync<T>(
                storedProcedureName,
                param,
                dbTransaction,
                commandTimeout,
                System.Data.CommandType.StoredProcedure);
        }

        public static Task<int> ExecuteStoredProcedureAsync(this SqlConnection sqlConnection, string storedProcedureName, object param = null, System.Data.IDbTransaction dbTransaction = null, int? commandTimeout = null)
        {
            return sqlConnection.ExecuteAsync(
                storedProcedureName,
                param,
                dbTransaction,
                commandTimeout,
                System.Data.CommandType.StoredProcedure);
        }

        public static IEnumerable<T> QueryStoredProcedure<T>(this SqlConnection sqlConnection, string storedProcedureName, object param = null, System.Data.IDbTransaction dbTransaction = null, bool buffered = true, int? commandTimeout = null)
        {
            return sqlConnection.Query<T>(storedProcedureName, param, dbTransaction, buffered, commandTimeout, System.Data.CommandType.StoredProcedure);
        }

        public static IEnumerable<dynamic> QueryStoredProcedure(this SqlConnection sqlConnection, string storedProcedureName, object param = null, System.Data.IDbTransaction dbTransaction = null, bool buffered = true, int? commandTimeout = null)
        {
            return sqlConnection.Query(storedProcedureName, param, dbTransaction, buffered, commandTimeout, System.Data.CommandType.StoredProcedure);
        }

        public static int ExecuteStoredProcedure(this SqlConnection sqlConnection, string storedProcedureName, object param = null, System.Data.IDbTransaction dbTransaction = null, int? commandTimeout = null)
        {
            return sqlConnection.Execute(storedProcedureName, param, dbTransaction, commandTimeout, System.Data.CommandType.StoredProcedure);
        }

    }
}
