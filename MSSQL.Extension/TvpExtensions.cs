using System.Collections.Generic;
using Microsoft.SqlServer.Server;

namespace MSSQL.Extension
{
    /// <summary>
    /// Extension methods for Table Valued Parameters
    /// </summary>
    public static class TvpExtensions
    {
        /// <summary>
        /// Convert an IEnumerable to a TvpList
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="customMetaData">Custom metadata to override default SqlMetaData generation</param>
        /// <returns></returns>
        public static TvpList<T> ToTvpList<T>(this IEnumerable<T> list, IEnumerable<SqlMetaData> customMetaData = null) where T : class
        {
            var tvpList = new TvpList<T>(customMetaData);
            tvpList.AddRange(list);
            return tvpList;
        }
    }
}
