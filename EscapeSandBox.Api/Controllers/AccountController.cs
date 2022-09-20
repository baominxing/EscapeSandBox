using EscapeSandBox.Api.Core;
using EscapeSandBox.Api.Domain;
using EscapeSandBox.Api.Dto;
using EscapeSandBox.Api.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EscapeSandBox.Api.Controllers
{
    [AllowAnonymous]
    [Route("Account")]
    public class AccountController : BaseController
    {
        private readonly ITokenHelper _tokenHelper;
        private readonly IDapperRepository<Agent, int> _userRepository;

        public AccountController(
            ITokenHelper tokenHelper,
            IDapperRepository<Agent, int> userRepository)
        {
            _tokenHelper = tokenHelper;
            _userRepository = userRepository;
        }

        [HttpPost]
        [Route("Login")]
        public R<dynamic> Login(LoginDto input)
        {
            var r = new R<dynamic>();

            try
            {
                Agent agent = _userRepository.FirstOrDefault(s => s.Code == input.UsernameOrEmail);

                if ((null != agent || input.UsernameOrEmail == ApiConfig.Admin) && input.Password == ApiConfig.DefaultPassword)
                {
                    r.Data = _tokenHelper.CreateToken(agent ?? new Agent { Code = ApiConfig.Admin, IpAddress = "127.0.0.1" });
                }
                else
                {
                    r.Status = EnumStatus.Failure;
                    r.Message = "账号或密码不正确";
                }
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
