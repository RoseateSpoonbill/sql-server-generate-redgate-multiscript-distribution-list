using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace GenerateMultiscriptList.Tests
{
    internal class Constants
    {

        public static string[] MultiScript_NodeTypeList = { "CNAME", "All" };
        public static string[] MultiScript_AuthTypeList = { "Windows", "SQL" };

        public static string[] MultiScript_DomainList_Valid = { "%.com" };

        public static string[] MultiScript_DatabaseTypeList_ExceptDBA = {
            "A",
            "AL",
            "B",
            "C",
            "E",
            "GA",
            "I",
            "R",
            "SSISDB",
            "V"};

        public static string[] MultiScript_EnvironmentCodeList_ExceptDev = { "T", "S", "P" };

        public static string[] MultiScript_SqlUserNamesList = { "DeployUser", "", "Example" };

        public static string[] AllOnly = { "All" };
        public static string[] CNameOnly = { "CNAME" };
        public static string[] DOnly = { "d" };
        public static string[] DbaOnly = { "DBA" };
        public static string[] NullStringOnly = { null };
        public static string[] SqlOnly = { "SQL" };
        public static string[] WindowsOnly = { "Windows" };

    }
}