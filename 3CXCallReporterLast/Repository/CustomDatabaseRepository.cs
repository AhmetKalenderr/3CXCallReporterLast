using System;
using System.Collections.Generic;
using System.Data;
using _3CXCallReporterLast.Helpers;
using _3CXCallReporterLast.Models;
using Npgsql;
using TCX.Configuration;

namespace _3CXCallReporterLast.Repository
{
    public class CustomDatabaseRepository
    {
        public string InsertData(List<CustomerForCSVModel> model)
        {
            NpgsqlConnection connectionFromPostgres = new NpgsqlConnection(GetConnectionStringClass.connFromPostgres);
            UpdateOlderData();
            connectionFromPostgres.Open();

            try
            {
                string guid = Guid.NewGuid().ToString();                

                string sql = $@"
                                INSERT INTO public.customers(
	                             ""customerName"", ""customerTc"", ""customerPhoneNumber"",""customerNote"",""customerPayment"",""lastInsertedData"",""lastUpdateTime"",""CreateDate"",""GroupGuid"")
	                            VALUES 
                ";
                DateTime insertTime = DateTime.Now.AddHours(1);

                foreach (var m in model)
                {
                    sql += $@"('{m.Name}','{m.TC}','{m.PhoneNumber}','{m.Note}','{m.Payment}',true,'{insertTime}','{insertTime}','{guid}'),";
                }
                sql = sql.Remove(sql.Length - 1);
                NpgsqlCommand command = new NpgsqlCommand(sql, connectionFromPostgres);

                command.ExecuteNonQuery();

                connectionFromPostgres.Close();

                return "Başarılı";

            }
            catch (System.Exception ex)
            {
                connectionFromPostgres.Close();
                Console.WriteLine(ex.Message);

                return "Başarısız";
            }

        }
        public CustomerForCSVModel GetDataByPhoneNumber(string phoneNumber)
        {
            NpgsqlConnection connectionFromPostgres = new NpgsqlConnection(GetConnectionStringClass.connFromPostgres);
            CustomerForCSVModel model = new CustomerForCSVModel();
            phoneNumber = phoneNumber.Substring(phoneNumber.Length-9);
            try
            {
                connectionFromPostgres.Open();
                string sql = $@"SELECT id, ""customerName"", ""customerTc"", ""customerPhoneNumber"", ""customerNote"", ""customerPayment""
	            FROM public.customers where ""customerPhoneNumber"" ilike '%{phoneNumber}%' limit 1";

                NpgsqlCommand cmd = connectionFromPostgres.CreateCommand();
                cmd.CommandText = sql;

                NpgsqlDataReader reader = cmd.ExecuteReader();
                
                while (reader.Read())
                {
                    model.Id = reader.GetInt32(0);
                    model.Name = reader.GetString(1);
                    model.TC = reader.GetString(2);
                    model.PhoneNumber = reader.GetString(3);
                    model.Note = reader.GetString(4);
                    model.Payment = reader.GetString(5);
                }

                connectionFromPostgres.Close();

            }
            catch (Exception ex)
            {
                connectionFromPostgres.Close();
                return model;
            }

            return model;


        }

        public int GetCountCustomerData()
        {
            NpgsqlConnection connectionFromPostgres = new NpgsqlConnection(GetConnectionStringClass.connFromPostgres);
            int count = 0;
            try
            {
                //Tğm data sayısı eklenecek
                connectionFromPostgres.Open();
                string sql = $@"SELECT count(*) FROM public.customers;";

                NpgsqlCommand cmd = connectionFromPostgres.CreateCommand();
                cmd.CommandText = sql;

                NpgsqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    count = reader.GetInt32(0);

                }

                connectionFromPostgres.Close();

            }
            catch (Exception ex)
            {
                connectionFromPostgres.Close();
            }

            return count;
        }


        public AgentModel GetAgentByAgentNumber(string agentNumber)
        {
            NpgsqlConnection connectionFromPostgres = new NpgsqlConnection(GetConnectionStringClass.connFromPostgres);

            AgentModel agent = new AgentModel();
            try
            {
                connectionFromPostgres.Open();
                string sql = $@"SELECT id, ""agentNumber"", ""agentPassword""
	        FROM public.agents where ""agentNumber"" = '{agentNumber}';";

                NpgsqlCommand cmd = connectionFromPostgres.CreateCommand();
                cmd.CommandText = sql;

                NpgsqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    agent.Id = reader.GetInt32(0);
                    agent.AgentNumber = reader.GetString(1);
                    agent.AgentPassword = reader.GetString(2);
                    
                }

                connectionFromPostgres.Close();

            }
            catch (Exception ex)
            {
                connectionFromPostgres.Close();
                return agent;
            }

            return agent;
        }

        public bool RegisterAgent(AgentModel registerModel)
        {
            NpgsqlConnection connectionFromPostgres = new NpgsqlConnection(GetConnectionStringClass.connFromPostgres);

            bool state = false;
            try
            {
                connectionFromPostgres.Open();
                string sql = $@"INSERT INTO public.agents(
	            ""agentNumber"", ""agentPassword"")
	            VALUES ('{registerModel.AgentNumber}', '{registerModel.AgentPassword}')"
                ;

                NpgsqlCommand command = new NpgsqlCommand(sql, connectionFromPostgres);

                command.ExecuteNonQuery();

                connectionFromPostgres.Close();

                state = true;


            }
            catch (Exception ex)
            {
               connectionFromPostgres.Close();
               state = false;
            }

            return state;
        }

        public bool UpdateNote(UpdateNoteCustomer phoneNumber)
        {
            NpgsqlConnection connectionFromPostgres = new NpgsqlConnection(GetConnectionStringClass.connFromPostgres);

            bool state = false;
            try
            {
                connectionFromPostgres.Open();
                string sql = $@"update  public.customers
                set ""customerNote"" = '{phoneNumber.Note}'
                where id =  {phoneNumber.Id}";

                NpgsqlCommand command = new NpgsqlCommand(sql, connectionFromPostgres);

                command.ExecuteNonQuery();

                connectionFromPostgres.Close();

                state = true;
                

            }
            catch (Exception ex)
            {
                connectionFromPostgres.Close();
                state = false;
            }

            return state;
        }


        public void UpdateOlderData()
        {
            NpgsqlConnection connectionFromPostgres = new NpgsqlConnection(GetConnectionStringClass.connFromPostgres);

            try
            {
                connectionFromPostgres.Open();
                string sql = $@"update  public.customers
                set ""lastUpdateTime"" = '{DateTime.Now.AddHours(1)}',
				""lastInsertedData"" = false
	            ;";

                NpgsqlCommand command = new NpgsqlCommand(sql, connectionFromPostgres);

                command.ExecuteNonQuery();

                connectionFromPostgres.Close();



            }
            catch (Exception ex)
            {
                connectionFromPostgres.Close();
            }

        }

        public bool DeleteLastInsertedData()
        {
            NpgsqlConnection connectionFromPostgres = new NpgsqlConnection(GetConnectionStringClass.connFromPostgres);

            bool state = false;
            try
            {
                connectionFromPostgres.Open();
                string sql = $@"DELETE FROM public.customers
	                    WHERE ""lastInsertedData"" = true;";

                NpgsqlCommand command = new NpgsqlCommand(sql, connectionFromPostgres);

                command.ExecuteNonQuery();

                connectionFromPostgres.Close();

                state = true;


            }
            catch (Exception ex)
            {
                connectionFromPostgres.Close();
                state = false;
            }

            return state;
        }

        public bool DeleteTodayInsertedDataCustomerData()
        {
            NpgsqlConnection connectionFromPostgres = new NpgsqlConnection(GetConnectionStringClass.connFromPostgres);

            bool state = false;
            try
            {
                //Bugün insert edilen dataları sil
                connectionFromPostgres.Open();
                string sql = $@"delete FROM public.customers
                where to_date(""CreateDate"",'DD-MM-YYYY') >= to_date('{DateTime.Now.AddHours(1)}','DD-MM-YYYY');";

                NpgsqlCommand command = new NpgsqlCommand(sql, connectionFromPostgres);

                command.ExecuteNonQuery();

                connectionFromPostgres.Close();

                state = true;


            }
            catch (Exception ex)
            {
                connectionFromPostgres.Close();
                state = false;
            }

            return state;
        }


        public bool DeleteAllData()
        {
            NpgsqlConnection connectionFromPostgres = new NpgsqlConnection(GetConnectionStringClass.connFromPostgres);

            bool state = false;
            try
            {
                connectionFromPostgres.Open();
                string sql = $@"DELETE FROM public.customers";

                NpgsqlCommand command = new NpgsqlCommand(sql, connectionFromPostgres);

                command.ExecuteNonQuery();

                connectionFromPostgres.Close();

                state = true;


            }
            catch (Exception ex)
            {
                connectionFromPostgres.Close();
                state = false;
            }

            return state;
        }


        public void CreateUserTableIfNotExists()
        {
            NpgsqlConnection connectionFromPostgres = new NpgsqlConnection(GetConnectionStringClass.connFromPostgres);

            try
            {
                connectionFromPostgres.Open();
                string sql = $@"CREATE TABLE IF NOT EXISTS public.customers
(
    id integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ),
    ""customerName"" character varying COLLATE pg_catalog.""default"",
    ""customerTc"" character varying COLLATE pg_catalog.""default"",
    ""customerPhoneNumber"" character varying COLLATE pg_catalog.""default"",
    ""customerNote"" character varying COLLATE pg_catalog.""default"",
    ""customerPayment"" character varying COLLATE pg_catalog.""default"",
    CONSTRAINT customers_pkey PRIMARY KEY (id)
)

TABLESPACE pg_default;
"
    ;
                NpgsqlCommand command = new NpgsqlCommand(sql, connectionFromPostgres);

                command.ExecuteNonQuery();

                connectionFromPostgres.Close();
            }
            catch (System.Exception ex)
            {
                connectionFromPostgres.Close();
                Console.WriteLine(ex.Message);
            }
        }
    }
}
