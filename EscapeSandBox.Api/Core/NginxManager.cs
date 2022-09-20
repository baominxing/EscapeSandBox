using EscapeSandBox.Api.Domain;
using EscapeSandBox.Api.Dto;
using EscapeSandBox.Api.Repository;

namespace EscapeSandBox.Api.Core
{
    public class NginxManager : INginxManager
    {
        private readonly object obj_look = new object();

        private readonly ILogger<NginxManager> _logger;
        private readonly IDapperRepository<Agent, int> _userRepository;
        private readonly IDapperRepository<AgentApp, int> _userAppRepository;

        public NginxManager(
            ILogger<NginxManager> logger,
            IDapperRepository<Agent, int> userRepository,
            IDapperRepository<AgentApp, int> userAppRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
            _userAppRepository = userAppRepository;
        }

        public void ReWrite()
        {
            try
            {
                var agentAppDtos = from up in _userAppRepository.GetAll()
                                   join u in _userRepository.GetAll() on up.Code equals u.Code
                                   select new AgentAppDto
                                   {
                                       Id = up.Id,
                                       Code = u.Code,
                                       IpAddress = u.IpAddress,
                                       ApplicationName = up.ApplicationName,
                                       ApplicationPort = up.ApplicationPort,
                                       ProxyPort = up.ProxyPort
                                   };

                lock (obj_look)
                {
                    // 读取nginx配置文件，进行重写
                    using (var ws = new StreamWriter(ApiConfig.NginxConfPath))
                    {
                        ws.WriteLine("user root;");
                        ws.WriteLine("worker_processes 1;");
                        ws.WriteLine("error_log /var/log/nginx/error.log warn;");
                        ws.WriteLine("pid /var/run/nginx.pid;");
                        ws.WriteLine("events {");
                        ws.WriteLine("    worker_connections 1024;");
                        ws.WriteLine("}");

                        ws.WriteLine("stream{");
                        foreach (var item in agentAppDtos)
                        {
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
            }
            catch (Exception ex)
            {
                _logger.LogError($"ReWrite Error : {ex.Message}");
            }
        }
    }
}
