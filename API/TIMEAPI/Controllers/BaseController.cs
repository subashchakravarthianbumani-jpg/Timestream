using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TIMEAPI.Controllers
{
    [ApiController]
    [Authorize]
    public class BaseController : ControllerBase
    {
        public BaseController() { }
    }
}
