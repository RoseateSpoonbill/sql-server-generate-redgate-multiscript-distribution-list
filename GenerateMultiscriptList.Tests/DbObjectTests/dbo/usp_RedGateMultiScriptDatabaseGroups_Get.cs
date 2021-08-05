using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using MSSQL.Extension;
using Dapper;
using FluentAssertions;
using Xunit;

namespace GenerateMultiscriptList.Tests.DbObjectTests.dbo
{

    [Collection("GenerateMultiscriptList")]
    public class usp_RedGateMultiScriptDatabaseGroups_Get : IClassFixture<DatabaseFixture>
    {
        private readonly DatabaseFixture fixture;
        private const string storedProcedureName = "[dbo].[usp_RedGateMultiScriptDatabaseGroups_Get]";

        public usp_RedGateMultiScriptDatabaseGroups_Get(DatabaseFixture fixture)
        {
            this.fixture = fixture;
        }

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

        public static XUnitExtension.Matrix5TheoryData<string, string, string, string, string> Parameters_ShouldNotThrow_Exception_Windows
            = new XUnitExtension.Matrix5TheoryData<string, string, string, string, string>(Constants.MultiScript_DomainList_Valid, Constants.Environment_Codes,
                Constants.Database_Types, Constants.SqlOnly, Constants.MultiScript_SqlUserNamesList);

        public static XUnitExtension.Matrix5TheoryData<string, string, string, string, string> Parameters_ShouldNotThrow_Exception_SQL
            = new XUnitExtension.Matrix5TheoryData<string, string, string, string, string>(Constants.MultiScript_DomainList_Valid, Constants.Environment_Codes,
                Constants.Database_Types, Constants.WindowsOnly, Constants.NullStringOnly);

        [Theory]
        [MemberData(nameof(Parameters_ShouldNotThrow_Exception_Windows))]
        [MemberData(nameof(Parameters_ShouldNotThrow_Exception_SQL))]
        [Trait("Category", "UnitTest")]
        public void ShouldReturn_UniqueGroupNames(string domain, string environmentCode, string databaseType, string authType, string sqlUserName)
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
                    EnvironmentCode = environmentCode,
                    DatabaseType = databaseType,
                    AuthType = authType,
                    SQLUserName = sqlUserName
                };

                List<ResultSet> actualResults = sqlConnection.QueryStoredProcedure<ResultSet>(storedProcedureName, parameters, transaction).AsList();

                List<string> repeatedGroupNames =
                    actualResults
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

                repeatedGroupNames.Should().BeEquivalentTo(expectedResults);
            }
        }

        [Theory]
        [MemberData(nameof(Parameters_ShouldNotThrow_Exception_Windows))]
        [MemberData(nameof(Parameters_ShouldNotThrow_Exception_SQL))]
        [Trait("Category", "UnitTest")]
        public void ShouldReturn_UniqueGroupGuids(string domain, string environmentCode, string databaseType, string authType, string sqlUserName)
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
                    EnvironmentCode = environmentCode,
                    DatabaseType = databaseType,
                    AuthType = authType,
                    SQLUserName = sqlUserName
                };

                List<ResultSet> actualResults = sqlConnection.QueryStoredProcedure<ResultSet>(storedProcedureName, parameters, transaction).AsList();

                List<string> repeatedGroupGuid =
                    actualResults
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

                repeatedGroupGuid.Should().BeEquivalentTo(expectedResults);
            }
        }

        [Theory]
        [MemberData(nameof(Parameters_ShouldNotThrow_Exception_Windows))]
        [MemberData(nameof(Parameters_ShouldNotThrow_Exception_SQL))]
        [Trait("Category", "UnitTest")]
        public void ShouldNotReturn_InvalidGroupNames(string domain, string environmentCode, string databaseType, string authType, string sqlUserName)
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
                    EnvironmentCode = environmentCode,
                    DatabaseType = databaseType,
                    AuthType = authType,
                    SQLUserName = sqlUserName
                };

                List<ResultSet> actualResults = sqlConnection.QueryStoredProcedure<ResultSet>(storedProcedureName, parameters, transaction)
                    .Where(i => i.GroupName == null || i.GroupName.Length < 10)
                    .AsList();

                // *******
                // ASSERT
                // *******

                actualResults.Should().BeEquivalentTo(expectedResults);
            }
        }

        [Theory]
        [MemberData(nameof(Parameters_ShouldNotThrow_Exception_Windows))]
        [MemberData(nameof(Parameters_ShouldNotThrow_Exception_SQL))]
        [Trait("Category", "UnitTest")]
        public void ShouldNotReturn_InvalidGroupGuids(string domain, string environmentCode, string databaseType, string authType, string sqlUserName)
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
                    EnvironmentCode = environmentCode,
                    DatabaseType = databaseType,
                    AuthType = authType,
                    SQLUserName = sqlUserName
                };

                List<ResultSet> actualResults = sqlConnection.QueryStoredProcedure<ResultSet>(storedProcedureName, parameters, transaction)
                    .Where(i => i.GroupGuid == null || i.GroupName.Length < 10)
                    .AsList();

                // *******
                // ASSERT
                // *******

                actualResults.Should().BeEquivalentTo(expectedResults);
            }
        }

        // --------------------------------------------------------------------------------------------------------------

        [Theory]
        [MemberData(nameof(Parameters_ShouldNotThrow_Exception_Windows))]
        [MemberData(nameof(Parameters_ShouldNotThrow_Exception_SQL))]
        [Trait("Category", "UnitTest")]
        public void ShouldReturn_Atleast1Record(string domain, string environmentCode, string databaseType, string authType, string sqlUserName)
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
                    EnvironmentCode = environmentCode,
                    DatabaseType = databaseType,
                    AuthType = authType,
                    SQLUserName = sqlUserName
                };

                List<ResultSet> actualResults = sqlConnection.QueryStoredProcedure<ResultSet>(storedProcedureName, parameters, transaction).AsList();

                // *******
                // ASSERT
                // *******

                actualResults.Count().Should().BeGreaterThan(0);
            }
        }

        // --------------------------------------------------------------------------------------------------------------

        public class ResultSet
        {
            public int GroupID { get; set; }
            public string EnvironmentCode { get; set; }
            public string DatabaseType { get; set; }
            public string AuthType { get; set; }
            public string NodeType { get; set; }
            public string GroupGuid { get; set; }
            public string GroupName { get; set; }
        }
    }
}
