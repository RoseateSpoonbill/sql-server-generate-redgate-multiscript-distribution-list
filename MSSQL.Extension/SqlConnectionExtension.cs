using System;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using Dapper.Contrib.Extensions;
using System.Collections.Generic;

namespace MSSQL.Extension
{
    public static class SqlConnectionExtension
    {



        /// <summary>
        /// Renames the current table to a new table name, and creates clone of that table without any constraints or indexes
        /// </summary>
        /// <param name="sqlConnection">Open SqlConnection to use</param>
        /// <param name="tableName">Two part name of table to mock</param>
        /// <param name="dbTransaction">SQL Transaction to use; use this if you want changes to automatically rollback</param>
        /// <param name="commandTimeout">Time (in seconds) for timeout if database execute does not complete</param>
        /// <param name="maintainIdentity">Whether or not we keep the identity column in the new table</param>
        /// <param name="maintainDefaultConstraint">Whether or not we keep the default constraints in the new table</param>
        /// <param name="maintainComputed">Whether or not we keep the computed column definitions in the new table</param>
        /// <returns></returns>
        public static int MockTable(this SqlConnection sqlConnection, string tableName, IDbTransaction dbTransaction = null, int? commandTimeout = null, bool maintainIdentity = true, bool maintainDefaultConstraint = true
            , bool maintainComputed = true)
        {
            string renameTableTo = "RollBackTable" + Guid.NewGuid().ToString().Substring(0, 6);

            sqlConnection.RenameDbObject(tableName, renameTableTo, out int objectID, dbTransaction, commandTimeout);

            string columnMetadataSql = @"
                SELECT 
                     c.[name]                                   AS ColumnName
                    ,c.column_id                                AS ColumnID
                    ,c.max_length                               AS ColumnMaxLength
                    ,c.[precision]                              AS [Precision]
                    ,c.scale                                    AS Scale
                    ,c.collation_name                           AS Collation
                    ,c.is_nullable                              AS IsNullable
                    ,IIF(identColumn.seed_value IS NULL, 0, 1)  AS IsIdentity
                    ,ISNULL(identColumn.seed_value, 0)          AS IdentitySeedValue
                    ,ISNULL(identColumn.increment_value, 0)     AS IdentityIncrementValue
                    ,defConstraint.DefaultConstraint            AS DefaultConstraint
                    ,t.[name]                                   AS TypeName
                    ,t.max_length                               AS TypeMaxLength
					, c.is_computed								AS IsComputed
					, cc.[definition]							AS ComputedDefinition
                FROM sys.columns AS c
                INNER JOIN sys.types AS t
                    ON c.system_type_id = t.system_type_id
                        AND c.user_type_id = t.user_type_id
                OUTER APPLY
                    (SELECT dc.definition AS DefaultConstraint
                        FROM sys.default_constraints AS dc
                        WHERE dc.object_id = c.default_object_id) AS defConstraint
                OUTER APPLY
                    (SELECT ic.seed_value,
                            ic.increment_value
                        FROM sys.identity_columns AS ic
                        WHERE ic.object_id = c.object_id
                            AND ic.column_id = c.column_id) AS identColumn
				LEFT JOIN sys.computed_columns AS cc
					ON C.object_id = cc.object_id
						AND c.column_id = cc.column_id
                WHERE c.object_id = @ObjectID
                ORDER BY c.column_id;";

            List<ColumnDefinition> columns = sqlConnection.Query<ColumnDefinition>(columnMetadataSql, new { ObjectID = objectID }, dbTransaction).AsList();

            string createTableSql = "";

            foreach (ColumnDefinition column in columns)
            {
                createTableSql += GetSqlDataTypeDefinition(column, maintainIdentity, maintainDefaultConstraint) + ", ";
            }

            createTableSql = createTableSql.Substring(0, createTableSql.Length - 1).Trim();

            createTableSql = "CREATE TABLE " + tableName + " (" + createTableSql + ");";

            return sqlConnection.Execute(createTableSql, param: null, dbTransaction);

        }

        public static int MockModule(this SqlConnection sqlConnection, string objectName, string mockObjectSqlDefinition, out string newObjectName, IDbTransaction dbTransaction = null, int? commandTimeout = null)
        {

            newObjectName = "RollBackModule" + Guid.NewGuid().ToString().Substring(0, 6);

            sqlConnection.RenameDbObject(objectName, newObjectName, dbTransaction, commandTimeout);

            return sqlConnection.Execute(mockObjectSqlDefinition, param: null, dbTransaction);

        }

        public static int MockModule(this SqlConnection sqlConnection, string objectName, string mockObjectSqlDefinition, IDbTransaction dbTransaction = null, int? commandTimeout = null)
        {
            return sqlConnection.MockModule(objectName, mockObjectSqlDefinition, out string newObjectName, dbTransaction, commandTimeout);
        }

        private static int RenameDbObject(this SqlConnection sqlConnection, string objectName, string newObjectName, out int objectID, IDbTransaction dbTransaction = null, int? commandTimeout = null)
        {
            objectName = objectName.Replace("[", "").Replace("]", "");

            string sql;

            sql = "SELECT OBJECT_ID(@ObjectName);";

            var parameters = new
            {
                ObjectName = objectName
            };

            int? nullableObjectID = sqlConnection.ExecuteScalar<int?>(sql, parameters, dbTransaction, commandTimeout, CommandType.Text);

            if (nullableObjectID == null)
            {
                throw new ArgumentException(String.Format("The object {0} does not exist", objectName));
            }
            else
            {
                objectID = nullableObjectID ?? 0;
            }

            sql = "EXEC sp_rename @objname = @objectName, @newname = @newObjectName, @objtype = 'OBJECT'";

            var renameParameters = new
            {
                objectName,
                newObjectName
            };

            return sqlConnection.Execute(sql, renameParameters, dbTransaction);
        }

        private static int RenameDbObject(this SqlConnection sqlConnection, string objectName, string newObjectName, IDbTransaction dbTransaction = null, int? commandTimeout = null)
        {
            return sqlConnection.RenameDbObject(objectName, newObjectName, out int objectID, dbTransaction, commandTimeout);
        }
        private static string GetSqlDataTypeDefinition(ColumnDefinition definition, bool maintainIdentity = false, bool maintainDefaultConstraint = false, bool maintainComputed = true)
        {
            string dataType = $"[{definition.ColumnName}]";

            if (definition.IsComputed && maintainComputed)
            {
                dataType += $" AS {definition.ComputedDefinition}";
            }
            else
            {
                dataType += $" [{definition.TypeName}]";

                // type length

                if (definition.TypeMaxLength == -1)
                {
                    dataType += "";
                }
                else if (definition.ColumnMaxLength == -1)
                {
                    dataType += "(MAX)";
                }
                else if (definition.TypeName.StartsWith("n") && definition.TypeName.Contains("char"))
                {
                    dataType += "(" + (definition.ColumnMaxLength / 2).ToString() + ")";
                }
                else if (definition.TypeName.Contains("char"))
                {
                        dataType += $"({definition.ColumnMaxLength.ToString()})";
                }
                else if (definition.TypeName == "decimal" || definition.TypeName == "numeric")
                {
                        dataType += $"({definition.Precision}, {definition.Scale})";
                }
                else
                {
                    dataType += "";
                }

                // collation
                if (definition.Collation != null)
                {
                        dataType += $" COLLATE {definition.Collation}";
                }

                // default constraints
                if (maintainDefaultConstraint && definition.DefaultConstraint != null)
                {
                        dataType += $" DEFAULT {definition.DefaultConstraint}";
                }

                // identity
                if (maintainIdentity && definition.IsIdentity &&
                    (definition.IdentitySeedValue != 0 || definition.IdentityIncrementValue != 0))
                {
                    dataType += " IDENTITY " + "(" + definition.IdentitySeedValue + ", " + definition.IdentityIncrementValue + ")";
                }

                dataType += " " + (definition.IsNullable ? "NULL" : "NOT NULL");
            }

            return dataType;
        }

        /// <summary>
        /// Does SET CONTEXT_INFO for the database connection
        /// </summary>
        /// <returns>Previous context info</returns>
        public static byte[] SetContextInfo(this SqlConnection sqlConnection, byte[] contextInfo, IDbTransaction transaction = null)
        {
            string sql = @"
                DECLARE @PreviousContextInfo VARBINARY(128) = CONTEXT_INFO();
                DECLARE @CurrentContextInfo VARBINARY(128) = ISNULL(CAST(@ContextInfo AS VARBINARY(128)),0x0);
                SET CONTEXT_INFO @CurrentContextInfo;
                SELECT @PreviousContextInfo;";

            var parameters = new
            {
                ContextInfo = contextInfo
            };

            return sqlConnection.ExecuteScalar<byte[]>(sql, parameters, transaction);
        }

        private class ColumnDefinition
        {
            public string ColumnName { get; set; }
            public int ColumnID { get; set; }
            public int ColumnMaxLength { get; set; }
            public int Precision { get; set; }
            public int Scale { get; set; }
            public string Collation { get; set; }
            public bool IsNullable { get; set; }
            public bool IsIdentity { get; set; }
            public long IdentitySeedValue { get; set; }
            public long IdentityIncrementValue { get; set; }
            public string DefaultConstraint { get; set; }
            public string TypeName { get; set; }
            public int TypeMaxLength { get; set; }
            public bool IsComputed { get; set; }
            public string ComputedDefinition { get; set; }
        }

    }
}