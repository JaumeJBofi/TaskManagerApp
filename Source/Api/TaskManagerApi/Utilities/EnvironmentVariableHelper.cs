using System.Text;

namespace TaskManagerApi.Utilities
{
    public static class EnvironmentVariableHelper
    {
        public static byte[] GetJwtKey()
        {                        
            //return Encoding.ASCII.GetBytes("ThisIsMySuperSecureSecretKey1234!");
            return Encoding.ASCII.GetBytes(GetValue("TM_JwtKey"));
        }

        public static string GetMongoDbConnectionString()
        {
            //return "mongodb://taskManagerAdmin:devPass1234!@localhost:27016/TaskManagerDb?authSource=admin";
            return GetValue("TM_MongoDbConnectionString");
        }

        public static string GetValue(string key)
        {
            string? value = Environment.GetEnvironmentVariable(key);
            return !string.IsNullOrEmpty(value) ? value : throw new InvalidOperationException($"{key} environment variable is not set.");        
        }
    }
}