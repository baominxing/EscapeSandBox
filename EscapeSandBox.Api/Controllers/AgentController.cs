using EscapeSandBox.Api.Core;
using EscapeSandBox.Api.Domain;
using EscapeSandBox.Api.Dto;
using EscapeSandBox.Api.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EscapeSandBox.Api.Controllers
{
    [Route("Agent")]
    public class AgentController : BaseController
    {
        private readonly ILogger<AgentController> _logger;
        private readonly IDapperRepository<Agent, int> _userRepository;
        private readonly INginxManager _nginxManager;

        public AgentController(
            ILogger<AgentController> logger,
            IDapperRepository<Agent, int> userRepository,
            INginxManager nginxManager)
        {
            _logger = logger;
            _userRepository = userRepository;
            _nginxManager = nginxManager;
        }

        [HttpPost]
        [Route("Update")]
        public R<dynamic> Update(AgentUpdateDto input)
        {
            _logger.LogInformation($"Update {JsonConvert.SerializeObject(input)}");

            var r = new R<dynamic>();

            try
            {
                var agent = _userRepository.FirstOrDefault(s => s.Code == input.Code);

                if (agent == null)
                {
                    throw new Exception($"注册码{input.Code}无效");
                }

                if (input.IpAddress == agent.IpAddress)
                {
                    throw new Exception($"注册码{input.Code}对应地址{input.IpAddress}没有改变");
                }

                _logger.LogInformation($"注册码{input.Code}对应地址{agent.IpAddress}更新为{input.IpAddress}");

                agent.IpAddress = input.IpAddress;

                _userRepository.Update(agent);

                _nginxManager.ReWrite();
            }
            catch (Exception ex)
            {
                r.Status = EnumStatus.Failure;
                r.Message = ex.Message;
            }

            return r;
        }
    }
}
