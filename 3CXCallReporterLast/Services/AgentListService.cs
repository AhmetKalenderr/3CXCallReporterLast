using _3CXCallReporterLast.Helpers;
using _3CXCallReporterLast.Models;
using _3CXCallReporterLast.Repository;
using Npgsql;
using System;
using System.Collections.Generic;
using TCX.Configuration;

namespace _3CXCallReporterLast.Services
{
    public class AgentListService
    {
        public List<AgentConnection> GetActiveConnectionAgent()
        {
            List<AgentConnection> detailConnList = new List<AgentConnection>();
            try
            {

                SingleDatabaseRepository singleDatabaseRepository = new SingleDatabaseRepository();
                List<AgentModel> agentList = singleDatabaseRepository.GetAllAgent();

                var call = PhoneSystem.Root.GetActiveConnectionsByCallID();
                CustomDatabaseRepository customRepo = new CustomDatabaseRepository();

                if (call != null)
                {
                    AgentConnection detailConn = new AgentConnection();
                    if (call.Count > 0)
                    {
                        foreach (var agent in agentList)
                        {
                            var a = 0;
                            foreach (var c in call.Values)
                            {


                                if (c[0].InternalParty != null)
                                {
                                    var state = c[0].InternalParty.ToString();
                                    var dn = c[0].DN.ToString();
                                    if (!state.Contains("Wexternalline"))
                                    {
                                        if (state.Contains(agent.AgentNumber) && c[0].Status.ToString().Contains("Connected"))
                                        {

                                            a = 1;
                                            //Arayan No => c[0].ExternalParty.ToString()

                                            detailConn.AgentNumber = agent.AgentNumber;
                                            detailConn.AgentName = agent.AgentName;
                                            detailConn.ConnectionTime = (DateTime.Now - c[0].LastChangeStatus.AddHours(1)).ToString();
                                            detailConn.ConnectionName = customRepo.GetDataByPhoneNumber(c[0].ExternalParty.ToString())?.Name;
                                            detailConn.ConnectionNumber = c[0].ExternalParty.ToString();
                                            detailConnList.Add(detailConn);

                                            break;
                                        }
                                    }
                                    else if (state.Contains("Wexternalline") && dn.Contains(agent.AgentNumber) && c[0].Status.ToString().Contains("Connected"))
                                    {
                                        a = 1;

                                        detailConn.AgentNumber = agent.AgentNumber;
                                        detailConn.AgentName = agent.AgentName;
                                        detailConn.ConnectionTime = (DateTime.Now - c[0].LastChangeStatus.AddHours(1)).ToString();
                                        detailConn.ConnectionName = customRepo.GetDataByPhoneNumber(c[0].ExternalParty.ToString())?.Name;
                                        detailConn.ConnectionNumber = c[0].ExternalParty.ToString();
                                        detailConnList.Add(detailConn);

                                        break;
                                    }
                                }
                            }
                            if (a != 1)
                            {

                                detailConn.AgentNumber = agent.AgentNumber;
                                detailConn.AgentName = agent.AgentName;
                                detailConn.ConnectionTime = "-";
                                detailConn.ConnectionNumber = "-";
                                detailConn.ConnectionName = "-";
                                detailConnList.Add(detailConn);
                            }

                        }

                    }
                    else
                    {
                        foreach (var agent in agentList)
                        {
                            AgentConnection detailConn2 = new AgentConnection();

                            detailConn2.AgentNumber = agent.AgentNumber;
                            detailConn2.AgentName = agent.AgentName;
                            detailConn2.ConnectionTime = "-";
                            detailConn2.ConnectionNumber = "-";
                            detailConn2.ConnectionName = "-";
                            detailConnList.Add(detailConn2);
                        }

                    }


                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }


            return detailConnList;
        }
    }
}
