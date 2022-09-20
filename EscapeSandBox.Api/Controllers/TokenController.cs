using EscapeSandBox.Api.Core;
using EscapeSandBox.Api.Domain;
using EscapeSandBox.Api.Dto;
using EscapeSandBox.Api.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EscapeSandBox.Api.Controllers
{
    [AllowAnonymous]
    [Route("Token")]
    public class TokenController : BaseController
    {
        private readonly ITokenHelper _tokenHelper;
        private readonly IDapperRepository<Agent, int> _userRepository;

        public TokenController(
            ITokenHelper tokenHelper,
            IDapperRepository<Agent, int> userRepository)
        {
            _tokenHelper = tokenHelper;
            _userRepository = userRepository;
        }

        [HttpPost]
        [Route("GetToken")]
        public IActionResult GetToken(GetTokenDto input)
        {
            Agent agent = _userRepository.FirstOrDefault(s => s.Code == input.Code);

            if (null != agent && input.Password == ApiConfig.DefaultPassword)
            {
                return Ok(_tokenHelper.CreateToken(agent));
            }

            return BadRequest();
        }
    }
}
