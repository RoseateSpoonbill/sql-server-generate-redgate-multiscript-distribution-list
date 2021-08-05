using System;

namespace MSSQL.Extension
{
    public static class StringExtensions
    {
        public static string Left(this string value, int maxLength)
        {
            return value.Substring(0, Math.Min(value.Length, maxLength));
        }
    }
}
