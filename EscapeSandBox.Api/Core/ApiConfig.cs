using Dapper;

namespace EscapeSandBox.Api.Core
{
    public class ApiConfig
    {
        public static string DatabaseConnectionString;

        public static string Issuer { get; set; }

        public static string Audience { get; set; }

        public static string IssuerSigningKey { get; set; }

        public static int AccessTokenExpiresMinutes { get; set; }

        public static string NginxConfPath { get; set; }

        public static string DefaultPassword { get; set; } = "123qwe";
        public static string Admin { get;  set; } = "admin";

        public const string DefaultCors = "EscapeSandBox";

        public void Initialize(IConfiguration configuration)
        {

            SimpleCRUD.SetDialect(SimpleCRUD.Dialect.SQLite);

            DatabaseConnectionString = configuration["App:DatabaseConnectionString"];

            Issuer = configuration["JWT:Issuer"];

            Audience = configuration["JWT:Audience"];

            IssuerSigningKey = configuration["JWT:IssuerSigningKey"];

            AccessTokenExpiresMinutes = Convert.ToInt32(configuration["JWT:AccessTokenExpiresMinutes"]);

            NginxConfPath = configuration["App:NginxConfPath"];
        }
    }
}
