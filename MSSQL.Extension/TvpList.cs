using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.SqlServer.Server;

namespace MSSQL.Extension
{
    public class TvpList<T> : List<T>, IEnumerable<SqlDataRecord> where T : class
    {
        private static Dictionary<Type, SqlDbType> _typeMap = new Dictionary<Type, SqlDbType>
        {
            { typeof(string), SqlDbType.NVarChar },
            //{ typeof(char), SqlDbType.NVarChar },
            { typeof(char[]), SqlDbType.NVarChar },
            { typeof(byte) , SqlDbType.TinyInt },
            { typeof(short), SqlDbType.SmallInt },
            { typeof(int), SqlDbType.Int },
            { typeof(long), SqlDbType.BigInt },
            { typeof(byte[]), SqlDbType.VarBinary },
            { typeof(bool), SqlDbType.Bit },
            { typeof(DateTime), SqlDbType.DateTime2 },
            { typeof(DateTimeOffset), SqlDbType.DateTimeOffset },
            { typeof(decimal), SqlDbType.Money },
            { typeof(float), SqlDbType.Real },
            { typeof(double), SqlDbType.Float },
            { typeof(TimeSpan), SqlDbType.Time },
        };

        private static Dictionary<SqlDbType, long> _defaultSizes = new Dictionary<SqlDbType, long>
        {
            { SqlDbType.NVarChar, 1000 },
            { SqlDbType.VarBinary, -1 }, //-1 = max
        };

        private IEnumerable<SqlMetaData> _metaData = null;

        public TvpList(IEnumerable<SqlMetaData> customMetaData = null) : base()
        {
            _metaData = customMetaData;
        }

        IEnumerator<SqlDataRecord> IEnumerable<SqlDataRecord>.GetEnumerator()
        {
            var metaData = GetMetaData().ToArray();

            var dataRecord = new SqlDataRecord(metaData);
            var properties = typeof(T).GetProperties();

            foreach (T data in this)
            {
                for (var i = 0; i < metaData.Length; i++)
                {
                    var md = metaData[i];
                    var prop = properties.FirstOrDefault(x => x.Name == md.Name) ??
                        throw new InvalidOperationException($@"Metadata name [{md.Name}] is not a valid property on type [{typeof(T).Name}]");
                    dataRecord.SetValue(i, prop.GetValue(data));
                }
                yield return dataRecord;
            }
        }

        private IEnumerable<SqlMetaData> GetMetaData()
        {
            if (_metaData != null)
            {
                return _metaData;
            }
            var metaData = new List<SqlMetaData>();
            var properties = typeof(T).GetProperties();
            foreach (var prop in properties)
            {
                var sqlDbType = GetSqlType(prop.PropertyType);
                var defaultSize = GetDefaultSize(sqlDbType);
                if (defaultSize != null)
                {
                    metaData.Add(new SqlMetaData(prop.Name, sqlDbType, defaultSize.Value));
                }
                else
                {
                    metaData.Add(new SqlMetaData(prop.Name, sqlDbType));
                }
            }
            _metaData = metaData;
            return _metaData;
        }

        /// <summary>
        /// Convert CLR types to SQLDB Types
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private SqlDbType GetSqlType(Type type)
        {
            // Allow nullable types to be handled
            var underlyingType = Nullable.GetUnderlyingType(type) ?? type;

            return _typeMap.ContainsKey(underlyingType) ? _typeMap[underlyingType] : throw new ArgumentException($"{type.FullName} is not a supported.");
        }

        private long? GetDefaultSize(SqlDbType sqlDbType)
        {
            return _defaultSizes.ContainsKey(sqlDbType) ? _defaultSizes[sqlDbType] : (long?)null;
        }
    }
}