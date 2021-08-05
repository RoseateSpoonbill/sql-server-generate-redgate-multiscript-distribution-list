using System;
using System.Data.SqlTypes;

namespace MSSQL.Extension
{
    public static class GenericConstants
    {

        #region Common Objects
        public static bool[] trueList = { true };
        public static bool[] falseList = { false };
        public static bool[] trueFalseList = { true, false };
        public static bool?[] boolValuesList = { true, false, null };

        public static DateTime maxSQLDate = DateTime.Parse("9999-12-31");
        public static DateTime maxSQLDateTime = DateTime.Parse("9999-12-31 23:59:59.997");

        public static string dateTimeFormat121 = "yyyy-MM-dd HH:mm:ss.fff"; // the same string format as SQL's CONVERT(VARCHAR(32), date, 121)

        public static string ticketNumber = "xxxx0000";

        public static int Convert_BytesToMB = 1024 * 1024;

        public static string[] Database_Types = { "A", "AL", "B", "C", "DBA", "E", "GA", "I", "R", "SSISDB", "V"};

        public static string[] Environment_Codes = { "D", "T", "S", "P" };
        public static string[] Environment_ShortNames = { "Dev", "Test", "Stage", "Prod" };

        public static string Environment_GetCodeFromShortName(string environmentShortName)
        {
            string environmentCode =
                environmentShortName == "Dev"
                ? "D"
                : environmentShortName == "Test"
                ? "T"
                : environmentShortName == "Stage"
                ? "S"
                : environmentShortName == "Prod"
                ? "P"
                : null;

            return environmentCode;
        }

        #endregion 

        #region Generic SQL

        public const string SqlGetTableCounts = @"
            SELECT 
	            TableName		= LOWER(CONCAT(SCHEMA_NAME(T.schema_id), '.', T.name))
	            , TotalRowCount = SUM(P.[rows])
            FROM [DBName].sys.tables AS T WITH (NOLOCK)
            INNER JOIN [DBName].sys.partitions AS P WITH (NOLOCK)
	            ON T.[object_id] = P.[object_id]
		            AND P.index_id IN ( 0, 1 )
            GROUP BY LOWER(CONCAT(SCHEMA_NAME(T.schema_id), '.', T.name));";


        #endregion
    }
}