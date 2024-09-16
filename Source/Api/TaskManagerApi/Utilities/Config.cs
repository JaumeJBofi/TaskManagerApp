namespace TaskManagerApi.Utilities
{
    public interface IConfig
    {
        public JwtSettings JwtSettings { get; }
        public string MongoDbConnectionString { get; }
        public string MongoDbDatabaseName { get; }
    }

    public class Config : IConfig
    {
        private readonly IConfiguration _configuration;

        public Config(IConfiguration configuration)
        {
            _configuration = configuration;

            JwtSettings = new JwtSettings(configuration.GetSection("JtwSettings"));
            MongoDbDatabaseName = configuration.GetValue<string>("MongoDbDatabaseName") ?? "TaskManagerDb";
            MongoDbConnectionString = configuration.GetValue<string>("MongoDbDatabaseName") ?? "TaskManagerDb";
        }

        public JwtSettings JwtSettings { get; }
        public string MongoDbConnectionString { get; }
        public string MongoDbDatabaseName { get; }
    }

    public class JwtSettings
    {
        public JwtSettings(IConfigurationSection configSection)
        {
            ValidAudience = configSection.GetValue<string>("ValidAudience") ?? throw new Exception("Audience not found");
            ValidIssuer = configSection.GetValue<string>("ValidIssuer") ?? throw new Exception("Issuer not found");
            AccessTokenExpirationMinutes = configSection.GetValue<int?>("AccessTokenExpirationMinutes") ?? 60;
            RefreshTokenExpirationMinutes = configSection.GetValue<int?>("RefreshTokenExpirationMinutes") ?? 1440;
        }
        public byte[] JwtKey { get { return EnvironmentVariableHelper.GetJwtKey(); } }
        public string ValidAudience { get; }
        public string ValidIssuer { get; }
        public int AccessTokenExpirationMinutes { get; }
        public int RefreshTokenExpirationMinutes { get; }
    }
}