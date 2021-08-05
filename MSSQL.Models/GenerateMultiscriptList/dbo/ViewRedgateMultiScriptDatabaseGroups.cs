using Dapper.Contrib.Extensions;

namespace MSSQL.Models.GenerateMultiscriptList.dbo
{
    [Table("dbo.View_RedgateMultiScriptDatabaseGroups")]
    public class ViewRedgateMultiScriptDatabaseGroups
    {
        public string EnvironmentCode { get; set; }
        public string DatabaseType { get; set; }
        public string NodeType { get; set; }
        public string AuthType { get; set; }
        public string SQLUserName { get; set; }
        public string GroupGuid { get; set; }
        public string GroupName { get; set; }
    }
}