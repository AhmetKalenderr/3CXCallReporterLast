using _3CXCallReporterLast.Models;
using _3CXCallReporterLast.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace _3CXCallReporterLast.Controllers
{
    [ApiController]
    public class AgentListController
    {
        AgentListService agentService = new AgentListService();

        [HttpPost("/getActiveAgentConnection")]
        public List<AgentConnection> GetActiveConnection()
        {
            return agentService.GetActiveConnectionAgent();
        }
    }
}
