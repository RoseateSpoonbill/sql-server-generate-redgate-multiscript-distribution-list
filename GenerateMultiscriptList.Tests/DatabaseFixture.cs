using System;
using System.Data.SqlClient;
using Xunit;


namespace GenerateMultiscriptList.Tests
{
    public class DatabaseFixture : IDisposable
    {
        public SqlConnection DbConnection { get; private set; }
        public DatabaseFixture()
        {
            string connectionString = Microsoft
               .Extensions
               .Configuration
               .ConfigurationExtensions
               .GetConnectionString(TestUtil.InitializeConfiguration(), "DefaultConnection");
            
            DbConnection = new SqlConnection(connectionString);
            DbConnection.Open();
        }

        public void Dispose()
        {
            DbConnection.Close();
            DbConnection = null;
        }

        
    }

    [CollectionDefinition("GenerateMultiscriptList")]
    public class DatabaseCollection : ICollectionFixture<DatabaseFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
