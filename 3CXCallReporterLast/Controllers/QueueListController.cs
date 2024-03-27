using _3CXCallReporterLast.Models;
using _3CXCallReporterLast.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace _3CXCallReporterLast.Controllers
{
    [ApiController]
    public class QueueListController
    {
        QueueListService queueService = new QueueListService();
        [HttpPost("/getActiveQueueConnection")]
        public List<QueueCustom> GetQueueListService()
        {
            return queueService.GetQueueWaitingList();
        }
    }
}
