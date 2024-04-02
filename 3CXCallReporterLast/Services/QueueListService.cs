using _3CXCallReporterLast.Helpers;
using _3CXCallReporterLast.Models;
using _3CXCallReporterLast.Repository;
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
            CustomDatabaseRepository customRepo = new CustomDatabaseRepository();

            List<QueueCustom> queueEntites = new List<QueueCustom>();
            SingleDatabaseRepository singleDatabaseRepo = new SingleDatabaseRepository();
            foreach (var queue in singleDatabaseRepo.GetAllQueue())
            {
                var queueCount = PhoneSystem.Root.GetDNByNumber(queue.QueueNumber).GetActiveConnections();

                if (PhoneSystem.Root.GetDNByNumber(queue.QueueNumber).GetActiveConnections().Length > 0)
                {

                    for (int i = 0; i < PhoneSystem.Root.GetDNByNumber(queue.QueueNumber).GetActiveConnections().Length; i++)
                    {
                        QueueCustom queueCustom = new QueueCustom();
                        queueCustom.QueueNumber = queue.QueueNumber;
                        queueCustom.QueueName = queue.QueueNumber;
                        queueCustom.WaitingNumber = queueCount[i]?.ExternalParty;
                        queueCustom.WaitingCustomerName = customRepo.GetDataByPhoneNumber(queueCount[i].ExternalParty)?.Name;
                        queueCustom.WaitingTime = (DateTime.Now - (queueCount[i].LastChangeStatus).AddHours(2)).ToString();
                        queueEntites.Add(queueCustom);
                    }
                }
                else
                {
                    QueueCustom queueCustom = new QueueCustom();
                    queueCustom.QueueNumber = queue.QueueNumber;
                    queueCustom.QueueName = queue.QueueNumber;
                    queueCustom.WaitingNumber = "-";
                    queueCustom.WaitingCustomerName = "-";
                    queueCustom.WaitingTime = "-";
                    queueEntites.Add(queueCustom);

                }
            }


            return queueEntites;
        }
    }
}
