using System.Web.Http;

namespace ResourceServer.Controllers
{
    [Authorize]
    public class MeController : ApiController
    {
        public string Get()
        {
            return this.User.Identity.Name;
        }

    }
}
