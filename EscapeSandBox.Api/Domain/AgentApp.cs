namespace EscapeSandBox.Api.Domain
{
    public class AgentApp : IEntity<int>
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public string IpAddress { get; set; }

        public string ApplicationName { get; set; }

        public int ApplicationPort { get; set; }

        public int ProxyPort { get; set; }
    }
}
