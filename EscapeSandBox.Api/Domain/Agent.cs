namespace EscapeSandBox.Api.Domain
{
    public class Agent : IEntity<int>
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public string IpAddress { get; set; }
    }
}
