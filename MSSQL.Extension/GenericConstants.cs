using System;
using System.Data.SqlTypes;

namespace MSSQL.Extension
{
    public static class GenericConstants
    {
        public static string[] Database_Types = { "A", "AL", "B", "C", "DBA", "E", "GA", "I", "R", "SSISDB", "V"};

        public static string[] Environment_Codes = { "D", "T", "S", "P" };
        public static string[] Environment_ShortNames = { "Dev", "Test", "Stage", "Prod" };
    }
}