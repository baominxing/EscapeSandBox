using Microsoft.Extensions.Configuration.Json;

namespace Agent
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IHost host = Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureAppConfiguration(e =>
                {
                    var configuration = e.SetBasePath(Directory.GetCurrentDirectory()).Add(new JsonConfigurationSource() { Path = "appsettings.json", ReloadOnChange = true }).Build();

                    new ConfigContent().Initialize(configuration);
                })
                .ConfigureLogging(log4net =>
                {
                    log4net.AddLog4Net("log4net.config");
                })
                .ConfigureServices(services =>
                {
                    services.AddHostedService<Worker>();
                })
                .Build();


            host.Run();
        }
    }
}