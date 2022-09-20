using EscapeSandBox.Api.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace EscapeSandBox.Api.Controllers
{
    [EnableCors(ApiConfig.DefaultCors)]
    [ApiController]
    [Authorize]
    public class BaseController : Controller
    {
    }
}
