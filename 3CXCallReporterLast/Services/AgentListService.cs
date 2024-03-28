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
            NpgsqlConnection connectionFromSingle = new NpgsqlConnection(GetConnectionStringClass.connFromSingle);

            connectionFromSingle.Open();

            var query = "SELECT dn as dn_number,display_name from users_view order by dn";

            NpgsqlCommand cmd = new NpgsqlCommand(query, connectionFromSingle);

            NpgsqlDataReader dr = cmd.ExecuteReader();

            List<AgentConnection> detailConnList = new List<AgentConnection>();



            var call = PhoneSystem.Root.GetActiveConnectionsByCallID();
            try
            {
                CustomDatabaseRepository customRepo = new CustomDatabaseRepository();

                if (call != null)
                {
                    if (call.Count > 0)
                    {
                        while (dr.Read())
                        {
                            AgentConnection detailConn = new AgentConnection();


                            var a = 0;
                            foreach (var c in call.Values)
                            {


                                if (c[0].InternalParty != null)
                                {
                                    var state = c[0].InternalParty.ToString();
                                    var dn = c[0].DN.ToString();
                                    if (!state.Contains("Wexternalline"))
                                    {
                                        if (state.Contains((string)dr["dn_number"]) && c[0].Status.ToString().Contains("Connected"))
                                        {

                                            a = 1;

                                            detailConn.AgentNumber = (string)dr["dn_number"];
                                            detailConn.AgentName = (string)dr["display_name"];
                                            detailConn.ConnectionTime = (DateTime.Now - c[0].LastChangeStatus.AddHours(3)).ToString();
                                            detailConn.ConnectionName = customRepo.GetDataByPhoneNumber(c[0].ExternalParty.ToString())?.Name;//Databaseden kullanıcı ismi çekilecek.
                                            detailConn.ConnectionNumber = c[0].ExternalParty.ToString();
                                            detailConnList.Add(detailConn);

                                            break;
                                        }
                                    }
                                    else if (state.Contains("Wexternalline") && dn.Contains((string)dr["dn_number"]) && c[0].Status.ToString().Contains("Connected"))
                                    {
                                        a = 1;

                                        detailConn.AgentNumber = (string)dr["dn_number"];
                                        detailConn.AgentName = (string)dr["display_name"];
                                        detailConn.ConnectionTime = (DateTime.Now - c[0].LastChangeStatus.AddHours(3)).ToString();
                                        detailConn.ConnectionName = customRepo.GetDataByPhoneNumber(c[0].ExternalParty.ToString())?.Name;//Databaseden kullanıcı ismi çekilecek.
                                        detailConn.ConnectionNumber = c[0].ExternalParty.ToString();
                                        detailConnList.Add(detailConn);

                                        break;
                                    }
                                }





                            }
                            if (a != 1)
                            {

                                detailConn.AgentNumber = (string)dr["dn_number"];
                                detailConn.AgentName = (string)dr["display_name"];
                                detailConn.ConnectionTime = "-";
                                detailConn.ConnectionNumber = "-";
                                detailConn.ConnectionName = "-";
                                detailConnList.Add(detailConn);
                            }
                        }

                    }
                    else
                    {
                        while (dr.Read())
                        {
                            AgentConnection detailConn2 = new AgentConnection();

                            detailConn2.AgentNumber = (string)dr["dn_number"];
                            detailConn2.AgentName = (string)dr["display_name"];
                            detailConn2.ConnectionTime = "-";
                            detailConn2.ConnectionNumber = "-";
                            detailConn2.ConnectionName = "-";
                            detailConnList.Add(detailConn2);

                        }

                    }

                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            connectionFromSingle.Close();

            return detailConnList;
        }
    }
}
