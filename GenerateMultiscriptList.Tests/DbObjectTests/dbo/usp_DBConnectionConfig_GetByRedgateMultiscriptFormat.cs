using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using MSSQL.Extension;
using Dapper;
using FizzWare.NBuilder;
using FluentAssertions;
using Xunit;

namespace GenerateMultiscriptList.Tests.DbObjectTests.dbo
{

    [Collection("GenerateMultiscriptList")]
    public class usp_DBConnectionConfig_GetByRedgateMultiscriptFormat : IClassFixture<DatabaseFixture>
    {
        private readonly DatabaseFixture fixture;
        private const string storedProcedureName = "[dbo].[usp_DBConnectionConfig_GetByRedgateMultiscriptFormat]";

        public usp_DBConnectionConfig_GetByRedgateMultiscriptFormat(DatabaseFixture fixture)
        {
            this.fixture = fixture;
        }

        // --------------------------------------------------------------------------------------------------------------

        public static string[] domainList_Valid = { "com" };

        public static XUnitExtension.Matrix3TheoryData<string, string, string> Parameters_Windows
            = new XUnitExtension.Matrix3TheoryData<string, string, string>(domainList_Valid, Constants.WindowsOnly, Constants.NullStringOnly);

        public static XUnitExtension.Matrix3TheoryData<string, string, string> Parameters_SQL
            = new XUnitExtension.Matrix3TheoryData<string, string, string>(domainList_Valid, Constants.SqlOnly, Constants.MultiScript_SqlUserNamesList);

        // --------------------------------------------------------------------------------------------------------------

        [Fact]
        [Trait("Category", "UnitTest")]
        public void ShouldThrow_Exception_WhenParametersAreInvalid()
        {
            // *******
            // ARRANGE
            // *******

            SqlConnection sqlConnection = fixture.DbConnection;

            List<string> expectedResults = new List<string> { };

            using (SqlTransaction transaction = sqlConnection.BeginTransaction())
            {
                // *******
                // ACT
                // *******

                var parameters = new
                {
                    Domain = (string)null,
                    EnvironmentCode = "A",
                    DatabaseType = "A",
                    AuthType = "A",
                    SQLUserName = "A"
                };

                Action act = () => sqlConnection.ExecuteStoredProcedure(storedProcedureName, parameters, transaction);

                // *******
                // ASSERT
                // *******
                act.Should()
                    .Throw<SqlException>()
                    .Where(exception => exception.Message.Contains("Invalid"));
            }
        }

        // --------------------------------------------------------------------------------------------------------------

        [Theory]
        [MemberData(nameof(Parameters_Windows))]
        [MemberData(nameof(Parameters_SQL))]
        [Trait("Category", "UnitTest")]
        public void ShouldReturn_FileStartAndEnd(string domain, string authType, string sqlUserName)
        {
            // *******
            // ARRANGE
            // *******

            SqlConnection sqlConnection = fixture.DbConnection;

            using (SqlTransaction transaction = sqlConnection.BeginTransaction())
            {
                // *******
                // ACT
                // *******

                var parameters = new
                {
                    Domain = domain,
                    EnvironmentCode = (string)null,
                    DatabaseType = (string)null,
                    AuthType = authType,
                    SQLUserName = sqlUserName,
                    TestRun = true
                };

                List<ResultSet_TestRunIsTrue> actualResults = sqlConnection.QueryStoredProcedure<ResultSet_TestRunIsTrue>(storedProcedureName, parameters, transaction)
                    .Where(i => i.Src.StartsWith("File"))
                    .AsList();

                List<ResultSet_TestRunIsTrue> expectedResults = Builder<ResultSet_TestRunIsTrue>
                    .CreateListOfSize(2)
                    .All() 
                    .TheFirst(1)
                    .And(i => i.TempID = 1)
                    .And(i => i.Src = "FileStart")
                    .TheLast(1)
                    .And(i => i.TempID = actualResults.Max(j => j.TempID))
                    .And(i => i.Src = "FileEnd")
                    .Build()
                    .AsList();

                // *******
                // ASSERT
                // *******

                actualResults.Should().BeEquivalentTo(expectedResults,
                    options => options
                        .Including(i => i.TempID)
                        .Including(i => i.Src)
                    );
            }
        }

        [Theory]
        [MemberData(nameof(Parameters_Windows))]
        [MemberData(nameof(Parameters_SQL))]
        [Trait("Category", "UnitTest")]
        public void ShouldReturn_GroupStartAndEnd(string domain, string authType, string sqlUserName)
        {
            // *******
            // ARRANGE
            // *******

            SqlConnection sqlConnection = fixture.DbConnection;

            using (SqlTransaction transaction = sqlConnection.BeginTransaction())
            {
                // *******
                // ACT
                // *******

                var parameters = new
                {
                    Domain = domain,
                    EnvironmentCode = (string)null,
                    DatabaseType = (string)null,
                    AuthType = authType,
                    SQLUserName = sqlUserName,
                    TestRun = true
                };

                List<ResultSet_TestRunIsTrue> actualResults = sqlConnection.QueryStoredProcedure<ResultSet_TestRunIsTrue>(storedProcedureName, parameters, transaction)
                    .Where(i => i.Src.StartsWith("Group"))
                    .AsList();

                var groupIds = actualResults.Select(i => i.GroupID).ToList().Distinct();
                List<int?> groupIdList = groupIds.ToList();

                List<ResultSet_TestRunIsTrue> expectedResults = Builder<ResultSet_TestRunIsTrue>
                    .CreateListOfSize(groupIds.Count() * 2)
                    .Build()
                    .AsList();

                for (int i = 0; i < groupIdList.Count(); i++)
                {
                    int groupId = (int)groupIdList[i];

                    expectedResults[i].GroupID = groupId;
                    expectedResults[i].Src = "GroupStart";
                    expectedResults[i + groupIdList.Count()].GroupID = groupId;
                    expectedResults[i + groupIdList.Count()].Src = "GroupEnd";
                }

                // *******
                // ASSERT
                // *******

                actualResults.Should().BeEquivalentTo(expectedResults, 
                    options => options
                        .Including(i => i.GroupID)
                        .Including(i => i.Src)
                        );
            }
        }


        [Theory]
        [MemberData(nameof(Parameters_Windows))]
        [MemberData(nameof(Parameters_SQL))]
        [Trait("Category", "UnitTest")]
        public void ShouldReturn_Results(string domain, string authType, string sqlUserName)
        {
            // *******
            // ARRANGE
            // *******

            SqlConnection sqlConnection = fixture.DbConnection;

            using (SqlTransaction transaction = sqlConnection.BeginTransaction())
            {
                // *******
                // ACT
                // *******

                var parameters = new
                {
                    Domain = domain,
                    EnvironmentCode = (string)null,
                    DatabaseType = (string)null,
                    AuthType = authType,
                    SQLUserName = sqlUserName,
                    TestRun = false
                };

                List<ResultSet_TestRunIsFalse> actualResults = sqlConnection.QueryStoredProcedure<ResultSet_TestRunIsFalse>(storedProcedureName, parameters, transaction).AsList();

                // *******
                // ASSERT
                // *******

                actualResults.Count().Should().BeGreaterThan(4);
            }
        }

        // --------------------------------------------------------------------------------------------------------------

        [Theory]
        [MemberData(nameof(Parameters_Windows))]
        [MemberData(nameof(Parameters_SQL))]
        [Trait("Category", "UnitTest")]
        public void ShouldReturn_UniqueGroupNames(string domain, string authType, string sqlUserName)
        {
            // *******
            // ARRANGE
            // *******

            SqlConnection sqlConnection = fixture.DbConnection;

            List<string> expectedResults = new List<string> { };

            using (SqlTransaction transaction = sqlConnection.BeginTransaction())
            {
                // *******
                // ACT
                // *******

                var parameters = new
                {
                    Domain = domain,
                    EnvironmentCode = (string)null,
                    DatabaseType = (string)null,
                    AuthType = authType,
                    SQLUserName = sqlUserName,
                    TestRun = true
                };

                List<ResultSet_TestRunIsTrue> actualResults = sqlConnection.QueryStoredProcedure<ResultSet_TestRunIsTrue>(storedProcedureName, parameters, transaction)
                    .AsList();

                List<string> repeatedGroupNames1 =
                    actualResults
                    .Where(i => i.Src == "GroupStart")
                    .GroupBy(i => i.GroupName)
                    .Select(i => new
                    {
                        GroupName = i.Key,
                        Num = i.Count()
                    })
                    .Where(i => i.Num > 1)
                    .Select(i => i.GroupName)
                    .AsList();

                List<string> repeatedGroupNames2 =
                    actualResults
                    .Where(i => i.Src == "GroupEnd")
                    .GroupBy(i => i.GroupName)
                    .Select(i => new
                    {
                        GroupName = i.Key,
                        Num = i.Count()
                    })
                    .Where(i => i.Num > 1)
                    .Select(i => i.GroupName)
                    .AsList();

                // *******
                // ASSERT
                // *******

                repeatedGroupNames1.Should().BeEquivalentTo(expectedResults);
                repeatedGroupNames2.Should().BeEquivalentTo(expectedResults);
            }
        }

        [Theory]
        [MemberData(nameof(Parameters_Windows))]
        [MemberData(nameof(Parameters_SQL))]
        [Trait("Category", "UnitTest")]
        public void ShouldReturn_UniqueGroupGuids(string domain, string authType, string sqlUserName)
        {
            // *******
            // ARRANGE
            // *******

            SqlConnection sqlConnection = fixture.DbConnection;

            List<string> expectedResults = new List<string> { };

            using (SqlTransaction transaction = sqlConnection.BeginTransaction())
            {
                // *******
                // ACT
                // *******

                var parameters = new
                {
                    Domain = domain,
                    EnvironmentCode = (string)null,
                    DatabaseType = (string)null,
                    AuthType = authType,
                    SQLUserName = sqlUserName,
                    TestRun = true
                };

                List<ResultSet_TestRunIsTrue> actualResults = sqlConnection.QueryStoredProcedure<ResultSet_TestRunIsTrue>(storedProcedureName, parameters, transaction)
                    .Where(i => i.GroupID != null || i.Src.StartsWith("Group"))
                    .AsList();

                List<string> repeatedGroupGuid1 =
                    actualResults
                    .Where(i => i.Src == "GroupStart")
                    .GroupBy(i => i.GroupGuid)
                    .Select(i => new
                    {
                        GroupGuid = i.Key,
                        Num = i.Count()
                    })
                    .Where(i => i.Num > 1)
                    .Select(i => i.GroupGuid)
                    .AsList();

                List<string> repeatedGroupGuid2 =
                    actualResults
                    .Where(i => i.Src == "GroupEnd")
                    .GroupBy(i => i.GroupGuid)
                    .Select(i => new
                    {
                        GroupGuid = i.Key,
                        Num = i.Count()
                    })
                    .Where(i => i.Num > 1)
                    .Select(i => i.GroupGuid)
                    .AsList();

                // *******
                // ASSERT
                // *******

                repeatedGroupGuid1.Should().BeEquivalentTo(expectedResults);
                repeatedGroupGuid2.Should().BeEquivalentTo(expectedResults);
            }
        }

        [Theory]
        [MemberData(nameof(Parameters_Windows))]
        [MemberData(nameof(Parameters_SQL))]
        [Trait("Category", "UnitTest")]
        public void ShouldNotReturn_InvalidGroupNames(string domain, string authType, string sqlUserName)
        {
            // *******
            // ARRANGE
            // *******

            SqlConnection sqlConnection = fixture.DbConnection;

            List<string> expectedResults = new List<string> { };

            using (SqlTransaction transaction = sqlConnection.BeginTransaction())
            {
                // *******
                // ACT
                // *******

                var parameters = new
                {
                    Domain = domain,
                    EnvironmentCode = (string)null,
                    DatabaseType = (string)null,
                    AuthType = authType,
                    SQLUserName = sqlUserName,
                    TestRun = true
                };

                List<ResultSet_TestRunIsTrue> actualResults = sqlConnection.QueryStoredProcedure<ResultSet_TestRunIsTrue>(storedProcedureName, parameters, transaction)
                    .Where(i => i.GroupID != null || i.Src.StartsWith("Group"))
                    .Where(i => i.GroupName == null || i.GroupName.Length < 10)
                    .AsList();

                // *******
                // ASSERT
                // *******

                actualResults.Should().BeEquivalentTo(expectedResults);
            }
        }

        [Theory]
        [MemberData(nameof(Parameters_Windows))]
        [MemberData(nameof(Parameters_SQL))]
        [Trait("Category", "UnitTest")]
        public void ShouldNotReturn_InvalidGroupGuids(string domain, string authType, string sqlUserName)
        {
            // *******
            // ARRANGE
            // *******

            SqlConnection sqlConnection = fixture.DbConnection;

            List<string> expectedResults = new List<string> { };

            using (SqlTransaction transaction = sqlConnection.BeginTransaction())
            {
                // *******
                // ACT
                // *******

                var parameters = new
                {
                    Domain = domain,
                    EnvironmentCode = (string)null,
                    DatabaseType = (string)null,
                    AuthType = authType,
                    SQLUserName = sqlUserName,
                    TestRun = true
                };

                List<ResultSet_TestRunIsTrue> actualResults = sqlConnection.QueryStoredProcedure<ResultSet_TestRunIsTrue>(storedProcedureName, parameters, transaction)
                    .Where(i => i.GroupID != null || i.Src.StartsWith("Group"))
                    .Where(i => i.GroupGuid == null || i.GroupName.Length < 10)
                    .AsList();

                // *******
                // ASSERT
                // *******

                actualResults.Should().BeEquivalentTo(expectedResults);
            }
        }

        // --------------------------------------------------------------------------------------------------------------

        public class ResultSet_TestRunIsTrue
        {
            public int TempID { get; set; }
            public string Src { get; set; }
            public int? GroupID { get; set; }
            public int? DatabaseID { get; set; }
            public string XMLCode { get; set; }
            public string G_EnvironmentCode { get; set; }
            public string G_DatabaseType { get; set; }
            public string AuthType { get; set; }
            public string NodeType { get; set; }
            public string GroupGuid { get; set; }
            public string GroupName { get; set; }
            public string D_EnvironmentCode { get; set; }
            public string D_DatabaseType { get; set; }
            public string DatabaseName { get; set; }
            public string CNamePrefix { get; set; }
            public string CNameSuffix { get; set; }
            public string Node1Prefix { get; set; }
            public string Node2Prefix { get; set; }
            public string Node3Prefix { get; set; }
            public string InstanceSuffix { get; set; }
            public string PortNumber { get; set; }
        }

        public class ResultSet_TestRunIsFalse
        {
            public string XMLCode { get; set; }
        }
    }
}
