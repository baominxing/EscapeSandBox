using EscapeSandBox.Api.Core;
using EscapeSandBox.Api.Domain;
using EscapeSandBox.Api.Dto;
using EscapeSandBox.Api.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EscapeSandBox.Api.Controllers
{
    [Route("AgentApp")]
    public class AgentAppController : BaseController
    {
        private readonly ILogger<AgentAppController> _logger;
        private readonly IDapperRepository<Agent, int> _userRepository;
        private readonly IDapperRepository<AgentApp, int> _userAppRepository;
        private readonly INginxManager _nginxManager;

        public AgentAppController(
            ILogger<AgentAppController> logger,
            IDapperRepository<Agent, int> userRepository,
            IDapperRepository<AgentApp, int> userAppRepository,
            INginxManager nginxManager)
        {
            _logger = logger;
            _userRepository = userRepository;
            _userAppRepository = userAppRepository;
            _nginxManager = nginxManager;
        }


        [HttpPost]
        [Route("GetAllAgent")]
        public R<dynamic> GetAllAgent(Agent input)
        {
            _logger.LogInformation($"GetAllAgent");

            var r = new R<dynamic>();

            try
            {
                var agentDtos = from up in _userRepository.GetAll()
                                select new Agent
                                {
                                    Id = up.Id,
                                    Code = up.Code,
                                    IpAddress = up.IpAddress,
                                };

                r.Data = agentDtos.Where(s => s.Code == input.Code || input.Code == ApiConfig.Admin);
            }
            catch (Exception ex)
            {
                r.Status = EnumStatus.Failure;
                r.Message = ex.Message;
            }

            return r;
        }

        [HttpPost]
        [Route("GetAll")]
        public R<dynamic> GetAll(Agent input)
        {
            _logger.LogInformation($"GetAll");

            var r = new R<dynamic>();

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

                r.Data = agentAppDtos.Where(s => s.Code == input.Code || input.Code == ApiConfig.Admin).OrderBy(s => s.Id);
            }
            catch (Exception ex)
            {
                r.Status = EnumStatus.Failure;
                r.Message = ex.Message;
            }

            return r;
        }

        [HttpPost]
        [Route("Get")]
        public R<dynamic> Get(AgentAppDto input)
        {
            _logger.LogInformation($"Get {JsonConvert.SerializeObject(input)}");

            var r = new R<dynamic>();

            try
            {
                var agentAppDto = (from up in _userAppRepository.GetAll()
                                   join u in _userRepository.GetAll() on up.Code equals u.Code
                                   select new AgentAppDto
                                   {
                                       Id = up.Id,
                                       Code = u.Code,
                                       IpAddress = u.IpAddress,
                                       ApplicationName = up.ApplicationName,
                                       ApplicationPort = up.ApplicationPort,
                                       ProxyPort = up.ProxyPort
                                   }).FirstOrDefault();

                r.Data = agentAppDto;
            }
            catch (Exception ex)
            {
                r.Status = EnumStatus.Failure;
                r.Message = ex.Message;
            }

            return r;
        }

        [HttpPost]
        [Route("Insert")]
        public R<dynamic> Insert(AgentAppDto input)
        {
            _logger.LogInformation($"Insert {JsonConvert.SerializeObject(input)}");

            var r = new R<dynamic>();

            try
            {
                var agent = _userRepository.FirstOrDefault(s => s.Code == input.Code);

                if (agent == null)
                {
                    throw new Exception($"注册码{input.Code}无效");
                }

                var entities = _userAppRepository.GetAll();

                if (entities.Any(s => s.Id != input.Id && s.ProxyPort == input.ProxyPort))
                {
                    throw new Exception($"注册码{input.Code}代理端口{input.ProxyPort}已被占用");
                }

                if (entities.Any(s => s.Id != input.Id && s.Code == input.Code && s.ApplicationName == input.ApplicationName && s.ApplicationPort == input.ApplicationPort))
                {
                    throw new Exception($"注册码{input.Code}应用程序{input.ApplicationName},端口{input.ApplicationPort}已被占用");
                }

                var entity = new AgentApp
                {
                    Code = input.Code,
                    ApplicationName = input.ApplicationName,
                    ApplicationPort = input.ApplicationPort,
                    IpAddress = input.IpAddress,
                    ProxyPort = input.ProxyPort
                };

                _userAppRepository.Insert(entity);

                _nginxManager.ReWrite();
            }
            catch (Exception ex)
            {
                r.Status = EnumStatus.Failure;
                r.Message = ex.Message;
            }

            return r;
        }

        [HttpPost]
        [Route("Delete")]
        public R<dynamic> Delete(AgentAppDto input)
        {
            _logger.LogInformation($"Delete {JsonConvert.SerializeObject(input)}");

            var r = new R<dynamic>();

            try
            {
                var agent = _userRepository.FirstOrDefault(s => s.Code == input.Code);

                if (agent == null)
                {
                    throw new Exception($"注册码{input.Code}无效");
                }

                var entity = _userAppRepository.FirstOrDefault(s => s.Id == input.Id);

                _userAppRepository.Delete(entity);

                _nginxManager.ReWrite();
            }
            catch (Exception ex)
            {
                r.Status = EnumStatus.Failure;
                r.Message = ex.Message;
            }

            return r;
        }

        [HttpPost]
        [Route("Update")]
        public R<dynamic> Update(AgentAppDto input)
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

                var entity = new AgentApp
                {
                    Id = input.Id,
                    Code = input.Code,
                    ApplicationName = input.ApplicationName,
                    ApplicationPort = input.ApplicationPort,
                    IpAddress = input.IpAddress,
                    ProxyPort = input.ProxyPort
                };

                _userAppRepository.Update(entity);

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
