using _3CXCallReporterLast.Models;
using _3CXCallReporterLast.Services;
using Microsoft.AspNetCore.Mvc;

namespace _3CXCallReporterLast.Controllers
{
    [ApiController]
    public class AgentController
    {
        [HttpPost("/RegisterAgent")]
        public RegisterResponseModel RegisterAgent([FromBody]AgentModel agent)
        {
            return new AgentService().RegisterAgent(agent);
        }
    }
}
