using _3CXCallReporterLast.Helpers;
using _3CXCallReporterLast.Models;
using Npgsql;
using System.Collections.Generic;
using System;
using TCX.Configuration;

namespace _3CXCallReporterLast.Services
{
    public class CardInfoService
    {
        public ResponseCardInfoModel GetCardInfoService()
        {
            NpgsqlConnection connectionFromPostgres = new NpgsqlConnection(GetConnectionStringClass.connFromSingle);
            ResponseCardInfoModel cardModel = new ResponseCardInfoModel();
            try
            {
                connectionFromPostgres.Open();
                string sql = $@"with recursive cte1 as (
	select
	*
	from
	cl_segments_view
	where start_time + interval '3 hour' between  date_trunc('day',now() + interval '3 hour') and  date_trunc('day',now() + interval '3 hour') + interval '1 day' and act != 1
	)
	,inb as (
	select
	*
	from (select day::date from generate_series(now() + interval '3 hour',now() + interval '3 hour',interval '1 day') day) d 
	left join (
		select
		date_trunc('day',start_time + interval '3 hour') as day,
		count(distinct(call_id)) as cnt
		from cte1
		where src_dn_type = 1
		and act != 15
		and act != 103
		group by date_trunc('day',start_time + interval '3 hour')
	) t using(day)	
	)
	, missed as (
	select
	*
	from (select day::date from generate_series( date_trunc('day',now() + interval '3 hour'), date_trunc('day',now() + interval '3 hour'),interval '1 day') day) d 
	left join (
		select
		date_trunc('day',start_time + interval '3 hour') as day,
		count(distinct(call_id)) as cnt
		from cte1
		where dst_dn_type = 4 and act != 10
		group by date_trunc('day',start_time + interval '3 hour')
	) t using(day)
)
select
coalesce(inb.cnt,0) as inbound_count,
coalesce(missed.cnt,0) as missed_count,
coalesce(inb.cnt-missed.cnt,0) as answered_count
from
inb
inner join missed on missed.day = inb.day
;";

                NpgsqlCommand cmd = connectionFromPostgres.CreateCommand();
                cmd.CommandText = sql;

                NpgsqlDataReader reader = cmd.ExecuteReader();

				int freeAgents = 0;
				Queue[] queues = PhoneSystem.Root.GetQueues();

                foreach (var queue in queues)
                {
					foreach (var agent in queue.Members)
					{
						Extension ext = (Extension)agent;
						if (ext.GetActiveConnections().Length ==0 && ext.QueueStatus == QueueStatusType.LoggedIn && ext.CurrentProfile.Name == "Available" && ext.IsRegistered)
						{
							freeAgents++;
						}
					}
                }

                while (reader.Read())
                {
					cardModel.TotalCallCount = reader.GetInt32(0).ToString();
					cardModel.AnsweredCallCount = reader.GetInt32(2).ToString();
					cardModel.UnAnsweredCallCount = reader.GetInt32(1).ToString();
					cardModel.AvailableAgentCount = freeAgents.ToString();
                }

                connectionFromPostgres.Close();

            }
            catch (Exception ex)
            {
                connectionFromPostgres.Close();
            }

            return cardModel;

        }
    }
}
