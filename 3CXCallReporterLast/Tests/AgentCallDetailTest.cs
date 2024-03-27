using _3CXCallReporterLast.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace _3CXCallReporterLast.Tesats
{
    [TestClass]
    public class AgentCallDetailTest
    {
        [TestMethod]
        public void AgentTest()
        {
            AgentConnection ac = new AgentConnection() { AgentName = "Ahmet", AgentNumber = "05432123123", ConnectionName = "Ali", ConnectionNumber = "asdasd", ConnectionTime = DateTime.Now.ToString(),Note = "Ahmet iyi",TC="12345689799" };
            Startup.setAgentCallDetail(ac);
        }
    }
}
