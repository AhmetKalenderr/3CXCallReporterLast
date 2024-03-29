namespace _3CXCallReporterLast.Models
{
    public class AgentRequestModel
    {
        public int Id { get; set; }
        public string agentDid { get; set; }
        public string callerNumber { get; set; }

        public string callerName { get; set; }

        public string tc { get; set; }

        public string note { get; set; }
        public string payment { get; set; }
    }
}
