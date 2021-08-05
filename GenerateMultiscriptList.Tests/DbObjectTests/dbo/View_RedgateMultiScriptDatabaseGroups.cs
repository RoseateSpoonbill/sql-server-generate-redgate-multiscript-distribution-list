using Dapper;
using FluentAssertions;
using System.Data.SqlClient;
using Xunit;
using System.Collections.Generic;
using MSSQL.Extension;
using System.Linq;
using System.Text.RegularExpressions;
using MSSQL.Models.GenerateMultiscriptList.dbo;
using FizzWare.NBuilder;
using MSSQL.Models;

namespace GenerateMultiscriptList.Tests.DbObjectTests.dbo
{
    [Collection("GenerateMultiscriptList")]
    public class View_RedgateMultiScriptDatabaseGroups : IClassFixture<DatabaseFixture>
    {
        private readonly DatabaseFixture fixture;
        private const string viewName = "[dbo].[View_RedgateMultiScriptDatabaseGroups]";

        string expectedGroupGuidFormat = "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx";

        public View_RedgateMultiScriptDatabaseGroups(DatabaseFixture fixture)
        {
            this.fixture = fixture;
        }

        // --------------------------------------------------------------------------------------------------------------
        public static string[] sqlUserNamesList = { "DeployUser", "" };

        // --------------------------------------------------------------------------------------------------------------
        [Fact]
        [Trait("Category", "UnitTest")]
        public void ShouldReturn_AllDatabaseTypes()
        {
            // *******
            // ARRANGE
            // *******

            SqlConnection sqlConnection = fixture.DbConnection;

            List<string> expectedResults = GenericConstants.Database_Types.AsList<string>();

            using (SqlTransaction transaction = sqlConnection.BeginTransaction())
            {
                // *******
                // ACT
                // *******

                string sql = @"
                SELECT DatabaseType
                FROM " + viewName + @"
                GROUP BY DatabaseType;";

                List<string> actualResults = sqlConnection.Query<string>(sql, null, transaction).AsList();

                // *******
                // ASSERT
                // *******

                actualResults.Should().BeEquivalentTo(expectedResults);
            }
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public void ShouldReturn_AllEnvironmentCodes()
        {
            // *******
            // ARRANGE
            // *******

            SqlConnection sqlConnection = fixture.DbConnection;

            List<string> expectedResults = GenericConstants.Environment_Codes.AsList<string>();

            using (SqlTransaction transaction = sqlConnection.BeginTransaction())
            {
                // *******
                // ACT
                // *******

                string sql = @"
                SELECT EnvironmentCode
                FROM " + viewName + @"
                GROUP BY EnvironmentCode;";

                List<string> actualResults = sqlConnection.Query<string>(sql, null, transaction).AsList();

                // *******
                // ASSERT
                // *******

                actualResults.Should().BeEquivalentTo(expectedResults);
            }
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public void ShouldReturn_AllAuthTypes()
        {
            // *******
            // ARRANGE
            // *******

            SqlConnection sqlConnection = fixture.DbConnection;

            List<string> expectedResults = Constants.MultiScript_AuthTypeList.AsList<string>();

            using (SqlTransaction transaction = sqlConnection.BeginTransaction())
            {
                // *******
                // ACT
                // *******

                string sql = @"
                SELECT AuthType
                FROM " + viewName + @"
                GROUP BY AuthType;";

                List<string> actualResults = sqlConnection.Query<string>(sql, null, transaction).AsList();

                // *******
                // ASSERT
                // *******

                actualResults.Should().BeEquivalentTo(expectedResults);
            }
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public void ShouldReturn_AllSQLUserNames()
        {
            // *******
            // ARRANGE
            // *******

            SqlConnection sqlConnection = fixture.DbConnection;

            List<string> expectedResults = sqlUserNamesList.AsList<string>();
            expectedResults.Add(null);

            using (SqlTransaction transaction = sqlConnection.BeginTransaction())
            {
                // *******
                // ACT
                // *******

                string sql = @"
                SELECT SQLUserName
                FROM " + viewName + @"
                GROUP BY SQLUserName;";

                List<string> actualResults = sqlConnection.Query<string>(sql, null, transaction).AsList();

                // *******
                // ASSERT
                // *******

                actualResults.Should().BeEquivalentTo(expectedResults);
            }
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public void ShouldReturn_AllNodeTypes()
        {
            // *******
            // ARRANGE
            // *******

            SqlConnection sqlConnection = fixture.DbConnection;

            List<string> expectedResults = Constants.MultiScript_NodeTypeList.AsList<string>();

            using (SqlTransaction transaction = sqlConnection.BeginTransaction())
            {
                // *******
                // ACT
                // *******

                string sql = @"
                SELECT NodeType
                FROM " + viewName + @"
                GROUP BY NodeType;";

                List<string> actualResults = sqlConnection.Query<string>(sql, null, transaction).AsList();

                // *******
                // ASSERT
                // *******

                actualResults.Should().BeEquivalentTo(expectedResults);
            }
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public void ShouldReturn_ProperlyFormattedGroupGuids()
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

                string sql = "SELECT * FROM " + viewName;

                List<ViewRedgateMultiScriptDatabaseGroups> actualResults = sqlConnection.Query<ViewRedgateMultiScriptDatabaseGroups>(sql, null, transaction).ToList();

                List<StringColumns_Two> expectedResultsList = new List<StringColumns_Two> { };
                List<StringColumns_Two> actualResultsList = new List<StringColumns_Two> { };

                foreach (ViewRedgateMultiScriptDatabaseGroups v in actualResults)
                {
                    string groupGuid = v.GroupGuid;

                    string newGroupGuid = Regex.Replace(groupGuid, @"[a-zA-Z0-9]", "*");
                    newGroupGuid = newGroupGuid.Replace("*", "x");

                    StringColumns_Two actualResult = new StringColumns_Two { String1 = groupGuid, String2 = newGroupGuid };
                    actualResultsList.Add(actualResult);

                    StringColumns_Two expectedResult = new StringColumns_Two { String1 = groupGuid, String2 = expectedGroupGuidFormat };
                    expectedResultsList.Add(expectedResult);
                }

                // *******
                // ASSERT
                // *******

                actualResultsList.Should().BeEquivalentTo(expectedResultsList);
            }
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public void ShouldReturn_GroupGuidsOfTheProperLength()
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

                string sql = @"
                SELECT GroupGuid
                FROM " + viewName + @"
                WHERE GroupGuid IS NULL
                    OR LEN(GroupGuid) <> " + expectedGroupGuidFormat.Length.ToString();

                List<string> actualResults = sqlConnection.Query<string>(sql, null, transaction).AsList();

                // *******
                // ASSERT
                // *******
                actualResults.Should().BeEmpty();
            }
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public void ShouldReturn_NoNullColumns_ExceptSQLUserName()
        {
            // SQLUserName is allowed to be null or empty string

            // *******
            // ARRANGE
            // *******

            SqlConnection sqlConnection = fixture.DbConnection;

            using (SqlTransaction transaction = sqlConnection.BeginTransaction())
            {
                // *******
                // ACT
                // *******

                string sql = @"
                SELECT *
                FROM " + viewName + @"
                WHERE EnvironmentCode IS NULL
                    OR EnvironmentCode = ''
                    OR DatabaseType IS NULL
                    OR DatabaseType = ''
                    OR NodeType IS NULL
                    OR NodeType = ''
                    OR AuthType IS NULL
                    OR AuthType = ''
                    OR GroupGuid IS NULL
                    OR GroupGuid = ''
                    OR GroupName IS NULL
                    OR GroupName = ''";

                List<ViewRedgateMultiScriptDatabaseGroups> actualResults = sqlConnection.Query<ViewRedgateMultiScriptDatabaseGroups>(sql, null, transaction).AsList();

                // *******
                // ASSERT
                // *******

                actualResults.Count().Should().Be(0);
            }
        }

        // --------------------------------------------------------------------------------------------------------------

        [Fact]
        [Trait("Category", "UnitTest")]
        public void ShouldReturn_UniqueGroupGuids()
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

                string sql = @"
                SELECT GroupGuid
                FROM " + viewName + @"
                GROUP BY GroupGuid
                HAVING COUNT(1) > 1;";

                List<string> actualResults = sqlConnection.Query<string>(sql, null, transaction).AsList();

                // *******
                // ASSERT
                // *******

                actualResults.Should().BeEquivalentTo(expectedResults);
            }
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public void ShouldReturn_UniqueRecords()
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

                string sql = @"
                SELECT EnvironmentCode, DatabaseType, AuthType, SQLUserName, NodeType
                FROM " + viewName + @"
                GROUP BY EnvironmentCode, DatabaseType, AuthType, SQLUserName, NodeType
                HAVING COUNT(1) > 1;";

                var actualResults = sqlConnection.Query(sql, null, transaction).AsList();

                // *******
                // ASSERT
                // *******

                actualResults.Count().Should().Be(0);
            }
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public void ShouldReturn_UniqueGroupNames()
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

                string sql = @"
                SELECT GroupName
                FROM " + viewName + @"
                GROUP BY GroupName
                HAVING COUNT(1) > 1;";

                List<string> actualResults = sqlConnection.Query<string>(sql, null, transaction).AsList();

                // *******
                // ASSERT
                // *******

                actualResults.Should().BeEquivalentTo(expectedResults);
            }
        }

        // --------------------------------------------------------------------------------------------------------------

        public static XUnitExtension.Matrix2TheoryData<string, string> Parameters_ShouldReturn_AtLeast1Result_WhereEnvironmentIsX_AndDatabaseTypeIsY
            = new XUnitExtension.Matrix2TheoryData<string, string>(GenericConstants.Environment_Codes, GenericConstants.Database_Types);

        [Theory]
        [MemberData(nameof(Parameters_ShouldReturn_AtLeast1Result_WhereEnvironmentIsX_AndDatabaseTypeIsY))]
        [Trait("Category", "UnitTest")]
        public void ShouldReturn_AtLeast1Result_WhereEnvironmentIsX_AndDatabaseTypeIsY(string environmentCode, string databaseType)
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

                string sql = @"
                SELECT COUNT(1) AS NumRows
                FROM " + viewName + @"
                WHERE DatabaseType = @DatabaseType
                    AND EnvironmentCode = @EnvironmentCode; ";

                var parameters = new
                {
                    DatabaseType = databaseType,
                    EnvironmentCode = environmentCode
                };

                int actualResults = sqlConnection.QueryFirst<int>(sql, parameters, transaction);

                // *******
                // ASSERT
                // *******

                actualResults.Should().BeGreaterOrEqualTo(1);
            }
        }

        public static XUnitExtension.Matrix5TheoryData<string, string, string, string, string> Parameters_ShouldReturn_1Result_AllNodeTypes_Windows
            = new XUnitExtension.Matrix5TheoryData<string, string, string, string, string>(Constants.MultiScript_EnvironmentCodeList_ExceptDev, GenericConstants.Database_Types, Constants.WindowsOnly, Constants.NullStringOnly, Constants.AllOnly);

        public static XUnitExtension.Matrix5TheoryData<string, string, string, string, string> Parameters_ShouldReturn_1Result_AllNodeTypes_SQL
            = new XUnitExtension.Matrix5TheoryData<string, string, string, string, string>(Constants.MultiScript_EnvironmentCodeList_ExceptDev, GenericConstants.Database_Types, Constants.SqlOnly, sqlUserNamesList, Constants.AllOnly);

        public static XUnitExtension.Matrix5TheoryData<string, string, string, string, string> Parameters_ShouldReturn_1Result_DBA_SQL
           = new XUnitExtension.Matrix5TheoryData<string, string, string, string, string>(Constants.DOnly, Constants.DbaOnly, Constants.SqlOnly, sqlUserNamesList, Constants.AllOnly);

        public static XUnitExtension.Matrix5TheoryData<string, string, string, string, string> Parameters_ShouldReturn_1Result_AnyAuth_Windows
            = new XUnitExtension.Matrix5TheoryData<string, string, string, string, string>(GenericConstants.Environment_Codes, Constants.MultiScript_DatabaseTypeList_ExceptDBA, Constants.WindowsOnly, Constants.NullStringOnly, Constants.CNameOnly);

        public static XUnitExtension.Matrix5TheoryData<string, string, string, string, string> Parameters_ShouldReturn_1Result_AnyAuth_SQL
            = new XUnitExtension.Matrix5TheoryData<string, string, string, string, string>(GenericConstants.Environment_Codes, Constants.MultiScript_DatabaseTypeList_ExceptDBA, Constants.SqlOnly, sqlUserNamesList, Constants.CNameOnly);

        [Theory]
        [MemberData(nameof(Parameters_ShouldReturn_1Result_AllNodeTypes_Windows))]
        [MemberData(nameof(Parameters_ShouldReturn_1Result_AllNodeTypes_SQL))]
        [InlineData("D", "DBA", "Windows", null, "All")]
        [MemberData(nameof(Parameters_ShouldReturn_1Result_DBA_SQL))]
        [MemberData(nameof(Parameters_ShouldReturn_1Result_AnyAuth_Windows))]
        [MemberData(nameof(Parameters_ShouldReturn_1Result_AnyAuth_SQL))]
        [Trait("Category", "UnitTest")]
        public void ShouldReturn_1Result_WhereEnvironmentIsA_AndDatabaseTypeIsB_AndAuthTypeIsC_AndNodeTypeIsD(string environmentCode, string databaseType,
            string authType, string sqlUserName, string nodeType)
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

                string sql = @"
                SELECT COUNT(1) AS NumRows
                FROM " + viewName + @"
                WHERE DatabaseType = @DatabaseType
                    AND EnvironmentCode = @EnvironmentCode
                    AND AuthType = @AuthType
                    AND NodeType = @NodeType
                    AND (SQLUserName = @SQLUserName
                        OR (SQLUserName IS NULL
                            AND @SQLUserName IS NULL
                            )
                        ); ";
                
                var parameters = new
                {
                    DatabaseType = databaseType,
                    EnvironmentCode = environmentCode,
                    AuthType = authType,
                    NodeType = nodeType,
                    SQLUserName = sqlUserName
                };

                int actualResults = sqlConnection.QueryFirst<int>(sql, parameters, transaction);

                // *******
                // ASSERT
                // *******

                actualResults.Should().Be(1);
            }
        }

        public static XUnitExtension.Matrix5TheoryData<string, string, string, string, string> Parameters_ShouldReturn_0Results_DBA_Windows
            = new XUnitExtension.Matrix5TheoryData<string, string, string, string, string>(GenericConstants.Environment_Codes, Constants.DbaOnly, Constants.WindowsOnly, Constants.NullStringOnly, Constants.CNameOnly);

        public static XUnitExtension.Matrix5TheoryData<string, string, string, string, string> Parameters_ShouldReturn_0Results_DBA_SQL
            = new XUnitExtension.Matrix5TheoryData<string, string, string, string, string>(GenericConstants.Environment_Codes, Constants.DbaOnly, Constants.SqlOnly, sqlUserNamesList, Constants.CNameOnly);

        public static XUnitExtension.Matrix5TheoryData<string, string, string, string, string> Parameters_ShouldReturn_0Results_AllNodeTypes_Windows
            = new XUnitExtension.Matrix5TheoryData<string, string, string, string, string>(Constants.DOnly, Constants.MultiScript_DatabaseTypeList_ExceptDBA, Constants.WindowsOnly, Constants.NullStringOnly, Constants.AllOnly);

        public static XUnitExtension.Matrix5TheoryData<string, string, string, string, string> Parameters_ShouldReturn_0Results_AllNodeTypes_SQL
            = new XUnitExtension.Matrix5TheoryData<string, string, string, string, string>(Constants.DOnly, Constants.MultiScript_DatabaseTypeList_ExceptDBA, Constants.SqlOnly, sqlUserNamesList, Constants.AllOnly);

        [Theory]
        [MemberData(nameof(Parameters_ShouldReturn_0Results_DBA_Windows))]
        [MemberData(nameof(Parameters_ShouldReturn_0Results_DBA_SQL))]
        [MemberData(nameof(Parameters_ShouldReturn_0Results_AllNodeTypes_Windows))]
        [MemberData(nameof(Parameters_ShouldReturn_0Results_AllNodeTypes_SQL))]
        [Trait("Category", "UnitTest")]
        public void ShouldReturn_0Results_WhereEnvironmentIsA_AndDatabaseTypeIsB_AndAuthTypeIsC_AndNodeTypeIsD(string environmentCode, string databaseType,
            string authType, string sqlUserName, string nodeType)
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

                string sql = @"
                SELECT COUNT(1) AS NumRows
                FROM " + viewName + @"
                WHERE DatabaseType = @DatabaseType
                    AND EnvironmentCode = @EnvironmentCode
                    AND AuthType = @AuthType
                    AND NodeType = @NodeType
                    AND(SQLUserName = @SQLUserName

                        OR(SQLUserName IS NULL

                            AND @SQLUserName IS NULL
                            )
                        ); ";

                var parameters = new
                {
                    DatabaseType = databaseType,
                    EnvironmentCode = environmentCode,
                    AuthType = authType,
                    NodeType = nodeType,
                    SQLUserName = sqlUserName
                };

                int actualResults = sqlConnection.QueryFirst<int>(sql, parameters, transaction);

                // *******
                // ASSERT
                // *******

                actualResults.Should().Be(0);
            }
        }

        // --------------------------------------------------------------------------------------------------------------

        public static XUnitExtension.Matrix1TheoryData<string> Parameters_ShouldReturn_AtLeast1Result_ForEachEnvironment
            = new XUnitExtension.Matrix1TheoryData<string>(GenericConstants.Environment_Codes);

        [Theory]
        [MemberData(nameof(Parameters_ShouldReturn_AtLeast1Result_ForEachEnvironment))]
        [Trait("Category", "UnitTest")]
        public void ShouldReturn_AtLeast1Result_ForEachEnvironment(string environmentCode)
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

                string sql = @"
                SELECT COUNT(1) AS NumRecords
                FROM " + viewName + @"
                WHERE EnvironmentCode = @EnvironmentCode;";

                var parameters = new
                {
                    EnvironmentCode = environmentCode
                };

                int actualResults = sqlConnection.QueryFirst<int>(sql, parameters, transaction);

                // *******
                // ASSERT
                // *******

                actualResults.Should().BeGreaterOrEqualTo(1);
            }
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public void ShouldReturn_SameDatabaseTypeAndEnvironmentCodeAsView_DBConnectionConfig()
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

                string sql = @"
		            SELECT 
			            ISNULL(X.EnvironmentCode, Y.EnvironmentCode) AS EnvironmentCode
			            , ISNULL(X.DatabaseType, Y.DatabaseType) AS DatabaseType
			            , CASE
                            WHEN X.EnvironmentCode IS NULL THEN 'Missing from View_DBConnectionConfig'
                            WHEN Y.EnvironmentCode IS NULL THEN 'Missing from View_RedgateMultiScriptDatabaseGroups'
                        END AS Error
		            FROM (
			            SELECT DISTINCT
				            EnvironmentCode
				            , DatabaseType
			            FROM dbo.View_DBConnectionConfigForRedgateMultiScript
			            ) AS X
		            FULL OUTER JOIN (
				            SELECT DISTINCT
					            EnvironmentCode
					            , DatabaseType
				            FROM dbo.View_RedgateMultiScriptDatabaseGroups
				            ) AS Y
			            ON X.EnvironmentCode = Y.EnvironmentCode
				            AND X.DatabaseType = Y.DatabaseType
                    WHERE X.EnvironmentCode IS NULL
                        OR Y.EnvironmentCode IS NULL;";

                var actualResults = sqlConnection.Query(sql, null, transaction).AsList();

                // *******
                // ASSERT
                // *******

                actualResults.Count().Should().Be(0);
            }
        }
    }
}