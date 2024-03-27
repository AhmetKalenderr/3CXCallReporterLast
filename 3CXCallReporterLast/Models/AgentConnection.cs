using System;

namespace _3CXCallReporterLast.Models
{
    public class AgentConnection
    {
        public string AgentName { get; set; }
        public string AgentNumber { get; set; }
        public string ConnectionNumber { get; set; }
        public string ConnectionTime { get; set; }
        public string ConnectionName { get; set; }
        public string AgentStatus { get; set; }
        public string LastChangeStatus { get; set; }
        public string Note { get; internal set; }
        public string TC { get; internal set; }
    }
}
