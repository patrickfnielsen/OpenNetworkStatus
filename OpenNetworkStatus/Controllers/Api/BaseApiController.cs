using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OpenNetworkStatus.Controllers.Api
{
    [ApiVersion("1.0")]
    [Authorize]
    [ApiController]
    public class BaseApiController : Controller
    {
        public string RequestedApiVersion => Request.HttpContext.GetRequestedApiVersion().ToString();
    }
}
