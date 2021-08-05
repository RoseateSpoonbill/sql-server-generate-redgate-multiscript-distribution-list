using Dapper;
using FluentAssertions;
using System.Data.SqlClient;
using Xunit;
using MSSQL.Extension;

namespace GenerateMultiscriptList.Tests.DbObjectTests.dbo
{
    [Collection("GenerateMultiscriptList")]
    public class View_DBConnectionConfig : IClassFixture<DatabaseFixture>
    {
        private readonly DatabaseFixture fixture;
        private const string viewName = "[dbo].[View_DBConnectionConfig]";

        public View_DBConnectionConfig(DatabaseFixture fixture)
        {
            this.fixture = fixture;
        }

        // --------------------------------------------------------------------------------------------------------------

        public static XUnitExtension.Matrix1TheoryData<string> Parameters_Environments
            = new XUnitExtension.Matrix1TheoryData<string>(Constants.Environment_ShortNames);

        [Theory]
        [MemberData(nameof(Parameters_Environments))]
        [Trait("Category", "UnitTest")]
        public void ShouldReturn_1NewAccountDB_ForEachEnvironment(string environment)
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
                SELECT COUNT(1) AS EnableNewAccountsDatabaseCount
                FROM dbo.View_DBConnectionConfig
                WHERE NewAccounts = 1
				    AND DeploymentSubGroup = @DeploymentSubGroup
                GROUP BY 
				    DeploymentSubGroup,
            	    ControlPlane,
            	    ConfigLevel;";

                    var parameters = new
                    {
                        DeploymentSubGroup = environment
                    };

                int enableNewAccountsDatabaseCount = sqlConnection.QueryFirst<int>(sql, parameters, transaction);

                // *******
                // ASSERT
                // *******

                enableNewAccountsDatabaseCount.Should().Be(1);
            }
        }
    }
}