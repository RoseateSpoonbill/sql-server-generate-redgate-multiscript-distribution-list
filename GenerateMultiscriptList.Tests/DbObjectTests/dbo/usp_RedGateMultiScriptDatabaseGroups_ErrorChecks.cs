using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using MSSQL.Extension;
using FluentAssertions;
using Xunit;

namespace GenerateMultiscriptList.Tests.DbObjectTests.dbo
{

    [Collection("GenerateMultiscriptList")]
    public class usp_RedGateMultiScriptDatabaseGroups_ErrorChecks : IClassFixture<DatabaseFixture>
    {
        private readonly DatabaseFixture fixture;
        private const string storedProcedureName = "[dbo].[usp_RedGateMultiScriptDatabaseGroups_ErrorChecks]";

        public usp_RedGateMultiScriptDatabaseGroups_ErrorChecks(DatabaseFixture fixture)
        {
            this.fixture = fixture;
        }

        // --------------------------------------------------------------------------------------------------------------

        public static string[] genericInvalidStringsList = { null, "A", "X" };

        public static string[] genericInvalidStringsList_ExcludeNull = { "A", "X" };

        // --------------------------------------------------------------------------------------------------------------

        public static string[] domainList_Invalid = { null, "com" };

        public static XUnitExtension.Matrix5TheoryData<string, string, string, string, string> Parameters_DomainIsInvalid_Windows
            = new XUnitExtension.Matrix5TheoryData<string, string, string, string, string>(domainList_Invalid, GenericConstants.Environment_Codes, 
                GenericConstants.Database_Types, Constants.SqlOnly, Constants.MultiScript_SqlUserNamesList);

        public static XUnitExtension.Matrix5TheoryData<string, string, string, string, string> Parameters_DomainIsInvalid_SQL
            = new XUnitExtension.Matrix5TheoryData<string, string, string, string, string>(domainList_Invalid, GenericConstants.Environment_Codes,
                GenericConstants.Database_Types, Constants.WindowsOnly, Constants.NullStringOnly);

        [Theory]
        [MemberData(nameof(Parameters_DomainIsInvalid_Windows))]
        [MemberData(nameof(Parameters_DomainIsInvalid_SQL))]
        [Trait("Category", "UnitTest")]
        public void ShouldThrow_Exception_WhenDomainIsInvalid(string domain, string environmentCode, string databaseType,
            string authType, string sqlUserName)
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

                Action act = () => sqlConnection.ExecuteStoredProcedure(storedProcedureName, parameters, transaction);

                // *******
                // ASSERT
                // *******
                act.Should()
                    .Throw<SqlException>()
                    .Where(exception => exception.Message.Contains("Invalid @Domain"));
            }
        }

        // --------------------------------------------------------------------------------------------------------------

        public static XUnitExtension.Matrix5TheoryData<string, string, string, string, string> Parameters_EnvironmentCodeIsInvalid_Windows
            = new XUnitExtension.Matrix5TheoryData<string, string, string, string, string>(Constants.MultiScript_DomainList_Valid, genericInvalidStringsList_ExcludeNull,
                GenericConstants.Database_Types, Constants.SqlOnly, Constants.MultiScript_SqlUserNamesList);

        public static XUnitExtension.Matrix5TheoryData<string, string, string, string, string> Parameters_EnvironmentCodeIsInvalid_SQL
            = new XUnitExtension.Matrix5TheoryData<string, string, string, string, string>(Constants.MultiScript_DomainList_Valid, genericInvalidStringsList_ExcludeNull,
                GenericConstants.Database_Types, Constants.WindowsOnly, Constants.NullStringOnly);

        [Theory]
        [MemberData(nameof(Parameters_EnvironmentCodeIsInvalid_Windows))]
        [MemberData(nameof(Parameters_EnvironmentCodeIsInvalid_SQL))]
        [Trait("Category", "UnitTest")]
        public void ShouldThrow_Exception_WhenEnvironmentCodeIsInvalid(string domain, string environmentCode, string databaseType,
            string authType, string sqlUserName)
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

                Action act = () => sqlConnection.ExecuteStoredProcedure(storedProcedureName, parameters, transaction);

                // *******
                // ASSERT
                // *******
                act.Should()
                    .Throw<SqlException>()
                    .Where(exception => exception.Message.Contains("Invalid @EnvironmentCode"));
            }
        }

        // --------------------------------------------------------------------------------------------------------------

        public static string[] databaseTypeList_Invalid = { "Documentation", "Sales" };

        public static XUnitExtension.Matrix5TheoryData<string, string, string, string, string> Parameters_DatabaseTypeIsInvalid_Windows
            = new XUnitExtension.Matrix5TheoryData<string, string, string, string, string>(Constants.MultiScript_DomainList_Valid, GenericConstants.Environment_Codes,
                databaseTypeList_Invalid, Constants.SqlOnly, Constants.MultiScript_SqlUserNamesList);

        public static XUnitExtension.Matrix5TheoryData<string, string, string, string, string> Parameters_DatabaseTypeIsInvalid_SQL
            = new XUnitExtension.Matrix5TheoryData<string, string, string, string, string>(Constants.MultiScript_DomainList_Valid, GenericConstants.Environment_Codes,
                databaseTypeList_Invalid, Constants.WindowsOnly, Constants.NullStringOnly);

        [Theory]
        [MemberData(nameof(Parameters_DatabaseTypeIsInvalid_Windows))]
        [MemberData(nameof(Parameters_DatabaseTypeIsInvalid_SQL))]
        [Trait("Category", "UnitTest")]
        public void ShouldThrow_Exception_WhenDatabaseTypeIsInvalid(string domain, string environmentCode, string databaseType,
            string authType, string sqlUserName)
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

                Action act = () => sqlConnection.ExecuteStoredProcedure(storedProcedureName, parameters, transaction);

                // *******
                // ASSERT
                // *******
                act.Should()
                    .Throw<SqlException>()
                    .Where(exception => exception.Message.Contains("Invalid @DatabaseType"));
            }
        }

        // --------------------------------------------------------------------------------------------------------------

        public static XUnitExtension.Matrix5TheoryData<string, string, string, string, string> Parameters_AuthTypeIsInvalid
            = new XUnitExtension.Matrix5TheoryData<string, string, string, string, string>(Constants.MultiScript_DomainList_Valid, GenericConstants.Environment_Codes,
                GenericConstants.Database_Types, genericInvalidStringsList, Constants.MultiScript_SqlUserNamesList);

        [Theory]
        [MemberData(nameof(Parameters_AuthTypeIsInvalid))]
        [Trait("Category", "UnitTest")]
        public void ShouldThrow_Exception_WhenAuthTypeIsInvalid(string domain, string environmentCode, string databaseType,
            string authType, string sqlUserName)
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

                Action act = () => sqlConnection.ExecuteStoredProcedure(storedProcedureName, parameters, transaction);

                // *******
                // ASSERT
                // *******
                act.Should()
                    .Throw<SqlException>()
                    .Where(exception => exception.Message.Contains("Invalid @AuthType"));
            }
        }

        // --------------------------------------------------------------------------------------------------------------

        public static XUnitExtension.Matrix5TheoryData<string, string, string, string, string> Parameters_SqlUserNameIsInvalid_Windows
            = new XUnitExtension.Matrix5TheoryData<string, string, string, string, string>(Constants.MultiScript_DomainList_Valid, GenericConstants.Environment_Codes,
                GenericConstants.Database_Types, Constants.WindowsOnly, Constants.MultiScript_SqlUserNamesList);

        [Theory]
        [MemberData(nameof(Parameters_SqlUserNameIsInvalid_Windows))]
        [Trait("Category", "UnitTest")]
        public void ShouldThrow_Exception_WhenSqlUserNameIsInvalidForWindowsAuth(string domain, string environmentCode, string databaseType,
            string authType, string sqlUserName)
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

                Action act = () => sqlConnection.ExecuteStoredProcedure(storedProcedureName, parameters, transaction);

                // *******
                // ASSERT
                // *******
                act.Should()
                    .Throw<SqlException>()
                    .Where(exception => exception.Message.Contains("@SQLUserName should be null for Windows auth"));
            }
        }


        public static XUnitExtension.Matrix5TheoryData<string, string, string, string, string> Parameters_SqlUserNameIsInvalid_SQL
            = new XUnitExtension.Matrix5TheoryData<string, string, string, string, string>(Constants.MultiScript_DomainList_Valid, GenericConstants.Environment_Codes,
                GenericConstants.Database_Types, Constants.SqlOnly, Constants.NullStringOnly);

        [Theory]
        [MemberData(nameof(Parameters_SqlUserNameIsInvalid_SQL))]
        [Trait("Category", "UnitTest")]
        public void ShouldThrow_Exception_WhenSqlUserNameIsInvalidForSQLAuth(string domain, string environmentCode, string databaseType,
            string authType, string sqlUserName)
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

                Action act = () => sqlConnection.ExecuteStoredProcedure(storedProcedureName, parameters, transaction);

                // *******
                // ASSERT
                // *******
                act.Should()
                    .Throw<SqlException>()
                    .Where(exception => exception.Message.Contains("Invalid @SQLUserName"));
            }
        }

        // --------------------------------------------------------------------------------------------------------------

        public static XUnitExtension.Matrix5TheoryData<string, string, string, string, string> Parameters_ShouldNotThrow_Exception_Windows
            = new XUnitExtension.Matrix5TheoryData<string, string, string, string, string>(Constants.MultiScript_DomainList_Valid, GenericConstants.Environment_Codes,
                GenericConstants.Database_Types, Constants.SqlOnly, Constants.MultiScript_SqlUserNamesList);

        public static XUnitExtension.Matrix5TheoryData<string, string, string, string, string> Parameters_ShouldNotThrow_Exception_SQL
            = new XUnitExtension.Matrix5TheoryData<string, string, string, string, string>(Constants.MultiScript_DomainList_Valid, GenericConstants.Environment_Codes,
                GenericConstants.Database_Types, Constants.WindowsOnly, Constants.NullStringOnly);

        [Theory]
        [MemberData(nameof(Parameters_ShouldNotThrow_Exception_Windows))]
        [MemberData(nameof(Parameters_ShouldNotThrow_Exception_SQL))]
        [Trait("Category", "UnitTest")]
        public void ShouldNotThrow_Exception(string domain, string environmentCode, string databaseType, string authType, string sqlUserName)
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

                Action act = () => sqlConnection.ExecuteStoredProcedure(storedProcedureName, parameters, transaction);

                // *******
                // ASSERT
                // *******
                act.Should().NotThrow<SqlException>();
            }
        }

    }
}
