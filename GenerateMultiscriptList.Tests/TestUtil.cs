using Microsoft.Extensions.Configuration;

namespace GenerateMultiscriptList.Tests
{
    public static class TestUtil
    {
        public static IConfiguration InitializeConfiguration()
        {
            return new ConfigurationBuilder()
                .AddJsonFile("appsettings.test.json")
                .Build();
        }
    }
}
