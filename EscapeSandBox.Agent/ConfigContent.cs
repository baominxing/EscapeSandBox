namespace Agent
{
    public class ConfigContent
    {
        public static string CacheKey => "EscapeSandBox";

        public static string AppPassword => "123qwe";

        public static int SlidingExpiration => 60 * 15;

        public static int FlurlApiTimeOut => 3000;

        public static string ApiUrl { get; set; }

        public static string AppId { get; set; }

        public static string IpPrefix { get; set; }

        public static int WorkerPeriod { get; set; }

        public void Initialize(IConfiguration configuration)
        {
            ApiUrl = configuration["App:ApiUrl"].ToString();

            AppId = configuration["App:AppId"].ToString();

            IpPrefix = configuration["App:IpPrefix"].ToString();

            WorkerPeriod = Convert.ToInt32(configuration["App:WorkerPeriod"].ToString());
        }
    }
}
