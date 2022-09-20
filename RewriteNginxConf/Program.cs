using Newtonsoft.Json;

namespace RewriteNginxConf
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var list = new List<AgentApp>()
            {
                new AgentApp{ IpAddress = "192.168.1.1", ApplicationPort=1, ProxyPort = 1 },
                new AgentApp{ IpAddress = "192.168.1.2", ApplicationPort=2, ProxyPort = 2 },
                new AgentApp{ IpAddress = "192.168.1.3", ApplicationPort=3, ProxyPort = 3 },
            };

            try
            {
                var filePath = "/root/escapesandbox/web/nginx.conf";

                Console.WriteLine(filePath);

                using (var ws = new StreamWriter(filePath))
                {
                    ws.WriteLine("user root;");
                    ws.WriteLine("worker_processes 1;");
                    ws.WriteLine("error_log /var/log/nginx/error.log warn;");
                    ws.WriteLine("pid /var/run/nginx.pid;");
                    ws.WriteLine("events {");
                    ws.WriteLine("    worker_connections 1024;");
                    ws.WriteLine("}");

                    ws.WriteLine("stream{");
                    foreach (var item in list)
                    {

                        Console.WriteLine(JsonConvert.SerializeObject(item));

                        ws.WriteLine("    server {");
                        ws.WriteLine($"        listen {item.ProxyPort};");
                        ws.WriteLine($"        proxy_pass {item.IpAddress}:{item.ApplicationPort};");
                        ws.WriteLine("    }");
                    }
                    ws.WriteLine("}");
                }

                Console.WriteLine("Rewrite Done");

                "down".DockerComposeBash();

                Console.WriteLine("docker-compose down");

                "up -d".DockerComposeBash();

                Console.WriteLine("docker-compose up");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            Console.ReadKey();
        }
    }

    public class AgentApp
    {
        public string IpAddress { get; set; }

        public ushort ApplicationPort { get; set; }

        public ushort ProxyPort { get; set; }
    }
}