using Dapper;
using FluentAssertions;
using System.Data.SqlClient;
using Xunit;
using GenerateMultiscriptList.Tests;

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

        [Theory]
        [InlineData("Dev")]
        [InlineData("Test")]
        [InlineData("Stage")]
        [InlineData("Prod")]
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

                // Get a list of all tables we expect to populate
                int enableNewAccountsDatabaseCount = GetEnableAccountsDatabaseCount(environment, sqlConnection, transaction);

                // *******
                // ASSERT
                // *******

                enableNewAccountsDatabaseCount.Should().Be(1);
            }
        }


        public int GetEnableAccountsDatabaseCount(string environment, SqlConnection sqlConnection, SqlTransaction transaction)
        {
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

            return enableNewAccountsDatabaseCount;
        }
    }
}