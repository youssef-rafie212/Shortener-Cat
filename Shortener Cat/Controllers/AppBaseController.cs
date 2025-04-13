using Microsoft.AspNetCore.Mvc;

namespace Shortener_Cat.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AppBaseController : ControllerBase
    {
    }
}
