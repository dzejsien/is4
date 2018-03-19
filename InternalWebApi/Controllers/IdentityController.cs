using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("identity")]
    [Authorize]
    public class IdentityController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return new JsonResult(from c in User.Claims select new { c.Type, c.Value });
        }

        [Route("getFromInternal")]
        [HttpGet]
        public IActionResult GetFromInternal()
        {
            return new JsonResult(new { result = "FROM INTERNAL RESPONSE SUPER TEST" });
        }
    }
}