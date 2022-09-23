using Agent.Core;
using Flurl.Http;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;

namespace Agent
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;

        public Worker(ILogger<Worker> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                _logger.LogInformation($"GetLocalIp : {GetLocalIp()}");

                await this.UpdateAgentIpAddress(GetLocalIp());

                await Task.Delay(TimeSpan.FromSeconds(ConfigContent.WorkerPeriod), stoppingToken);
            }
        }

        private string GetLocalIp()
        {
            ///获取本地的IP地址
            string ipAddress = string.Empty;

            foreach (var item in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if (item.AddressFamily == AddressFamily.InterNetwork && item.ToString().StartsWith(ConfigContent.IpPrefix))
                {
                    ipAddress = item.ToString();
                }
            }
            return ipAddress;
        }

        private async Task UpdateAgentIpAddress(string ipAddress)
        {
            try
            {
                var token = MemoryCacheHelper.GetOrAddCacheItem(ConfigContent.CacheKey, ConfigContent.SlidingExpiration, GetToken);

                if (string.IsNullOrEmpty(token.TokenContent))
                {
                    _logger.LogInformation($"Get Token From Cahce Is Null Or Empty");

                    MemoryCacheHelper.Remove(ConfigContent.CacheKey);

                    return;
                }

                var response = await $"{ConfigContent.ApiUrl.TrimEnd('/')}/Agent/Update"
                    .WithOAuthBearerToken(token.TokenContent)
                    .WithTimeout(TimeSpan.FromSeconds(ConfigContent.FlurlApiTimeOut))
                    .PostJsonAsync(new AgentUdpateDto { Code = ConfigContent.AppId, IpAddress = ipAddress })
                    .ReceiveJson<R<dynamic>>();

                if (response.Status != EnumStatus.Success)
                {
                    throw new Exception(response.Message);
                }

                _logger.LogInformation($"UpdateAgentIpAddress Done. Agent:{ConfigContent.AppId},IpAddress:{ipAddress}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"UpdateAgentIpAddress Error : {ex}");
            }
        }

        private TokenDto GetToken()
        {

            try
            {
                var token_url = $"{ConfigContent.ApiUrl.TrimEnd('/')}/Token/GetToken";

                var token_result = token_url
                    .WithTimeout(TimeSpan.FromMilliseconds(ConfigContent.FlurlApiTimeOut))
                    .PostJsonAsync(new
                    {
                        Code = ConfigContent.AppId,
                        Password = ConfigContent.AppPassword
                    }).ReceiveJson<R<TokenDto>>().Result;


                _logger.LogInformation($"GetToken {JsonConvert.SerializeObject(token_result)}");

                return token_result.Data;
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetToken Error:{ex}");
            }

            return new TokenDto();
        }
    }

    public class GetTokenDto
    {
        public string Code { get; set; }

        public string Password { get; set; }
    }

    public class TokenDto
    {
        public string TokenContent { get; set; }

        public DateTime Expires { get; set; }
    }

    public class AgentUdpateDto
    {
        public string Code { get; set; }

        public string IpAddress { get; set; }
    }

    public class ResponseDto
    {
        public bool Success { get; set; }

        public string Message { get; set; }

        public object Data { get; set; }
    }

    public class R<T>
    {
        /// <summary>
        /// 请求状态
        /// </summary>
        public EnumStatus Status { get; set; } = EnumStatus.Success;

        private string? _msg;

        /// <summary>
        /// 消息描述
        /// </summary>
        public string Message
        {
            get { return !string.IsNullOrEmpty(_msg) ? _msg : EnumHelper.GetEnumDescription(Status); }
            set { _msg = value; }
        }

        /// <summary>
        /// 返回结果
        /// </summary>
        public T? Data { get; set; }
    }

    public enum EnumStatus
    {
        [Description("请求成功")]
        Success = 1,
        [Description("请求失败")]
        Failure = 0,
        [Description("请求异常")]
        Unknown = -1,

    }
}