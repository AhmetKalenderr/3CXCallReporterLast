using _3CXCallReporterLast.Helpers;
using _3CXCallReporterLast.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using TCX.Configuration;

namespace _3CXCallReporterLast.Services
{
    public class QueueListService
    {
        public List<QueueCustom> GetQueueWaitingList()
        {
            NpgsqlConnection connectionFromSingle = new NpgsqlConnection(GetConnectionStringClass.connFromSingle);

            connectionFromSingle.Open();

            var query = "Select dn,display_name from queue_view";

            List<QueueCustom> queueEntites = new List<QueueCustom>();

            NpgsqlCommand cmd = new NpgsqlCommand(query, connectionFromSingle);
            NpgsqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                var queueCount = PhoneSystem.Root.GetDNByNumber((string)dr["dn"]).GetActiveConnections();

                if (PhoneSystem.Root.GetDNByNumber((string)dr["dn"]).GetActiveConnections().Length > 0)
                {

                    for (int i = 0; i < PhoneSystem.Root.GetDNByNumber((string)dr["dn"]).GetActiveConnections().Length; i++)
                    {
                        QueueCustom queue = new QueueCustom();
                        queue.QueueNumber = (string)dr["dn"];
                        queue.QueueName = (string)dr["display_name"];
                        queue.WaitingNumber = queueCount[i].ExternalParty;
                        queue.WaitingTime = (DateTime.Now - (queueCount[i].LastChangeStatus).AddHours(3)).ToString();
                        queueEntites.Add(queue);
                    }
                }
                else
                {
                    QueueCustom queue = new QueueCustom();
                    queue.QueueNumber = (string)dr["dn"];
                    queue.QueueName = (string)dr["display_name"];
                    queue.WaitingNumber = "-";
                    queue.WaitingTime = "-";
                    queueEntites.Add(queue);

                }
            }
            connectionFromSingle.Close();


            return queueEntites;
        }
    }
}
