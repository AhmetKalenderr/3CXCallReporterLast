using _3CXCallReporterLast.Helpers;
using _3CXCallReporterLast.Models;
using Npgsql;
using System.Collections.Generic;

namespace _3CXCallReporterLast.Repository
{
    public class SingleDatabaseRepository
    {
        public List<AgentModel> GetAllAgent()
        {
            NpgsqlConnection connectionFromSingle = new NpgsqlConnection(GetConnectionStringClass.connFromSingle);
            List<AgentModel> allAgents = new List<AgentModel>();

            connectionFromSingle.Open();

            var query = "SELECT dn as dn_number,display_name from users_view order by dn";

            NpgsqlCommand cmd = new NpgsqlCommand(query, connectionFromSingle);

            NpgsqlDataReader dr = cmd.ExecuteReader();

            while(dr.Read())
            {
                allAgents.Add(new AgentModel
                {
                    AgentNumber = dr["dn_number"].ToString(),
                    AgentName = dr["display_name"].ToString()
                }) ;
            }

            connectionFromSingle.Close();

            return allAgents;
        }


        public List<QueueModel> GetAllQueue()
        {
            NpgsqlConnection connectionFromSingle = new NpgsqlConnection(GetConnectionStringClass.connFromSingle);

            connectionFromSingle.Open();

            var query = "Select dn,display_name from queue_view";
            NpgsqlCommand cmd = new NpgsqlCommand(query, connectionFromSingle);

            NpgsqlDataReader dr = cmd.ExecuteReader();

            List<QueueModel> allQueues = new List<QueueModel>();

            while (dr.Read())
            {
                allQueues.Add(new QueueModel
                {
                    QueueNumber = dr["dn"].ToString(),
                    QueueName = dr["display_name"].ToString()
                });
            }

            connectionFromSingle.Close();

            return allQueues;
        }
    }
}
