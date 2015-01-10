using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Http;

namespace WebApiImplicitGrantFlow
{
    [Route("demo")]
    [Authorize]
    public class DemoController : ApiController
    {
        public string Get()
        {
            var name = "anonymous";

            var nameClaim = ((ClaimsPrincipal)User).FindFirst(ClaimTypes.Name);
            if (nameClaim != null)
            {
                name = nameClaim.Value;
            }

            return "The user is: " + name;
        }
    }
}