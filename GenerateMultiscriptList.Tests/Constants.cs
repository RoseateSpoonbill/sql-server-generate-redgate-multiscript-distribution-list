namespace GenerateMultiscriptList.Tests
{
    internal class Constants
    {
		// should change
        public static string[] Database_Types = { "DB1", "DB2", "DB3" };
        public static string[] MultiScript_SqlUserNamesList = { "DeployUser", "", "Example" };

		// might need to change depending on your setup
        public static string[] MultiScript_DomainList_Valid = { "%.com" };
        public static string[] Environment_Codes = { "D", "T", "S", "P" };
        public static string[] Environment_ShortNames = { "Dev", "Test", "Stage", "Prod" };
        public static string[] MultiScript_NodeTypeList = { "CNAME", "All" };

		// should not change
        public static string[] MultiScript_AuthTypeList = { "Windows", "SQL" };
        public static string[] NullStringOnly = { null };
        public static string[] SqlOnly = { "SQL" };
        public static string[] WindowsOnly = { "Windows" };

    }
}