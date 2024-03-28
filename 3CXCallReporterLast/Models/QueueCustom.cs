using System.Collections.Concurrent;
using System.Collections.Generic;
using System;

namespace _3CXCallReporterLast.Models
{
    public class QueueCustom
    {
        public string QueueName { get; set; }
        public string QueueNumber { get; set; }
        public string WaitingNumber { get; set; }
        public string WaitingTime { get; set; }

        public string WaitingCustomerName { get; set; }
    }
}
