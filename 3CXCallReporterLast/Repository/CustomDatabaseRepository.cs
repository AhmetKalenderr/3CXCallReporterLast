using _3CXCallReporterLast.Helpers;
using _3CXCallReporterLast.Models;
using Npgsql;
using System;
using System.Collections.Generic;

namespace _3CXCallReporterLast.Repository
{
    public class CustomDatabaseRepository
    {
        public CsvInsertDataResponseModel InsertData(List<CustomerForCSVModel> model)
        {
            NpgsqlConnection connectionFromPostgres = new NpgsqlConnection(GetConnectionStringClass.connFromPostgres);
            UpdateOlderData();
            connectionFromPostgres.Open();
            CsvInsertDataResponseModel modelResponse = new CsvInsertDataResponseModel();

            try
            {
                string guid = Guid.NewGuid().ToString();

                string sqlForQuinn = $@"
                                INSERT INTO public.customers(
	                             ""customerName"", ""customerTc"", ""customerPhoneNumber"",""customerNote"",""customerPayment"",""lastInsertedData"",""lastUpdateTime"",""CreateDate"",""GroupGuid"")
	                            VALUES 
                ";

                string sqlForWe = $@"
                                INSERT INTO public.""OurCustomerData""(
	                             ""name"", ""identityNumber"", ""phoneNumber"",""customerNote"",""customerPayment"",""lastInsertedData"",""lastUpdateTime"",""CreateDate"",""GroupGuid"")
	                            VALUES 
                ";
                DateTime insertTime = DateTime.Now.AddHours(1);

                foreach (var m in model)
                {
                    //string formatDataNumber = m.PhoneNumber.Substring(m.PhoneNumber.Length - 10);
                    //if (!formatDataNumber.StartsWith('5'))
                    //{
                    //    modelResponse.success = false;
                    //    modelResponse.message = "Data formatı hatalı.";
                    //    return modelResponse;
                    //}
                    sqlForQuinn += $@"('{m.Name}','{m.TC}','{m.PhoneNumber}','{m.Note}','{m.Payment}',true,'{insertTime}','{insertTime}','{guid}'),";
                    sqlForWe += $@"('{m.Name}','{m.TC}','{m.PhoneNumber}','{m.Note}','{m.Payment}',true,'{insertTime}','{insertTime}','{guid}'),";
                }
                sqlForQuinn = sqlForQuinn.Remove(sqlForQuinn.Length - 1);
                sqlForWe = sqlForWe.Remove(sqlForWe.Length - 1);
                NpgsqlCommand command = new NpgsqlCommand(sqlForQuinn, connectionFromPostgres);
                NpgsqlCommand command2 = new NpgsqlCommand(sqlForWe, connectionFromPostgres);

                command.ExecuteNonQuery();
                command2.ExecuteNonQuery();

                connectionFromPostgres.Close();

                modelResponse.success = true;
                modelResponse.message = "Başarılı";
                return modelResponse;

            }
            catch (System.Exception ex)
            {
                connectionFromPostgres.Close();
                Console.WriteLine(ex.Message);
                modelResponse.message = "Başarısız";
                modelResponse.success= false;
                return modelResponse;
            }

        }
        public CustomerForCSVModel GetDataByPhoneNumber(string phoneNumber)
        {
            NpgsqlConnection connectionFromPostgres = new NpgsqlConnection(GetConnectionStringClass.connFromPostgres);
            CustomerForCSVModel model = new CustomerForCSVModel();
            phoneNumber = phoneNumber.Substring(phoneNumber.Length - 9);
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

        public List<GroupCsvModel> GetGroupCsv()
        {
            NpgsqlConnection connectionFromPostgres = new NpgsqlConnection(GetConnectionStringClass.connFromPostgres);
            List<GroupCsvModel> listGroupCsv = new List<GroupCsvModel>();
            try
            {
                connectionFromPostgres.Open();
                string sql = $@"SELECT 
	            count(*) as ""DataCount"",
	            ""CreateDate"",
	            ""GroupGuid""
	            FROM public.customers 
	            where length(""GroupGuid"")>1
	            group by ""GroupGuid"",""CreateDate""
	            order by ""CreateDate"" desc
	            ;";

                NpgsqlCommand cmd = connectionFromPostgres.CreateCommand();
                cmd.CommandText = sql;

                NpgsqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    GroupCsvModel groupCsvModel = new GroupCsvModel
                    {
                        DataCount = reader.GetInt32(0),
                        CreateDate = reader.GetString(1),
                        GroupGuid = reader.GetString(2)
                    };

                    listGroupCsv.Add(groupCsvModel);

                }

                connectionFromPostgres.Close();

            }
            catch (Exception ex)
            {
                connectionFromPostgres.Close();
            }

            return listGroupCsv;
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

        public bool DeleteDataByGuid(string guid)
        {
            NpgsqlConnection connectionFromPostgres = new NpgsqlConnection(GetConnectionStringClass.connFromPostgres);

            bool state = false;
            try
            {
                connectionFromPostgres.Open();
                string sql = $@"DELETE FROM public.customers
	                    WHERE ""GroupGuid"" = '{guid}';";

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

        public List<CustomerForCSVModel> GetGroupCsvDetails(string guid)
        {
            NpgsqlConnection connectionFromPostgres = new NpgsqlConnection(GetConnectionStringClass.connFromPostgres);
            List<CustomerForCSVModel> listCustomerCsv = new List<CustomerForCSVModel>();
            try
            {
                connectionFromPostgres.Open();
                string sql = $@"select ""id"",
	        ""customerName"",
	        ""customerPhoneNumber"",
	        ""customerTc"",
	        ""customerNote"" from public.customers
            where ""GroupGuid"" = '{guid}';";

                NpgsqlCommand cmd = connectionFromPostgres.CreateCommand();
                cmd.CommandText = sql;

                NpgsqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    CustomerForCSVModel csvDetail = new CustomerForCSVModel
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        PhoneNumber = reader.GetString(2),
                        TC = reader.GetString(3),
                        Note = reader.GetString(4),
                    };

                    listCustomerCsv.Add(csvDetail);

                }

                connectionFromPostgres.Close();

            }
            catch (Exception ex)
            {
                connectionFromPostgres.Close();
            }

            return listCustomerCsv;

        }

        public bool DeleteDataById(string id)
        {
            bool successState = false;
            NpgsqlConnection connectionFromPostgres = new NpgsqlConnection(GetConnectionStringClass.connFromPostgres);

            try
            {
                connectionFromPostgres.Open();
                string sql = $@"DELETE FROM public.customers
	                    WHERE ""id"" = '{id}';";

                NpgsqlCommand command = new NpgsqlCommand(sql, connectionFromPostgres);

                command.ExecuteNonQuery();

                connectionFromPostgres.Close();

                successState = true;


            }
            catch (Exception ex)
            {
                connectionFromPostgres.Close();
                successState = false;
            }

            return successState;
            return successState;
           
        }

        public DialerOpenOrCloseModel CheckPaymentAndOpenOrCloseDialer(bool state)
        {
            NpgsqlConnection connectionFromPostgres = new NpgsqlConnection(GetConnectionStringClass.connFromPostgres);

            Boolean PaymentState = false;
            DialerOpenOrCloseModel model = new DialerOpenOrCloseModel();
            try
            {
                connectionFromPostgres.Open();
                string sql = $@"select ""isDialerPayment"" from public.""DialerTable"";";
                NpgsqlCommand cmd = connectionFromPostgres.CreateCommand();
                cmd.CommandText = sql;

                NpgsqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    PaymentState = reader.GetBoolean(0);
                }
                reader.Close();

                if (PaymentState)
                {
                    string sqlOpenState = $@"update public.""DialerTable"" set ""isDialerOpen"" = {state} where id = 1;";

                    cmd.CommandText = sqlOpenState;

                    cmd.ExecuteNonQuery();
                    model.Message = "İşlem Başarılı";
                    model.Status = true;                    
                }
                else
                {
                    model.Message = "Ödeme Yapılmadığı için Dialer başlatılamadı.";
                    model.Status = false;
                }

                connectionFromPostgres.Close();

            }
            catch (Exception ex)
            {
                connectionFromPostgres.Close();
            }

            return model;
        }

        public string ChangeState(bool state)
        {
            NpgsqlConnection connectionFromPostgres = new NpgsqlConnection(GetConnectionStringClass.connFromPostgres);
            string stateMesssage = "";
            connectionFromPostgres.Open();

            if (state)
            {

                string sqlOpenState = $@"update public.""DialerTable"" set ""isDialerPayment"" = {state} where id = 1;";

                NpgsqlCommand command = new NpgsqlCommand(sqlOpenState, connectionFromPostgres);

                command.ExecuteNonQuery();
                stateMesssage = "Ödeme yapıldı alanı evet olarak set edildi";

            } else
            {
                string sqlOpenState = $@"update public.""DialerTable"" set ""isDialerPayment"" = {state} , ""isDialerOpen"" = {state} where id = 1;";

                NpgsqlCommand command = new NpgsqlCommand(sqlOpenState, connectionFromPostgres);

                command.ExecuteNonQuery();
                stateMesssage = "Şimdi koyduk amlarına";
            }
            connectionFromPostgres.Close();
            return stateMesssage;


            
        }

        public bool CheckDialerState()
        {
            NpgsqlConnection connectionFromPostgres = new NpgsqlConnection(GetConnectionStringClass.connFromPostgres);
            bool dialerState = false;
            try
            {
                connectionFromPostgres.Open();
                string sql = $@"select ""isDialerOpen"" from public.""DialerTable"";";
                NpgsqlCommand cmd = connectionFromPostgres.CreateCommand();
                cmd.CommandText = sql;

                NpgsqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    dialerState = reader.GetBoolean(0);
                }


                connectionFromPostgres.Close();

            }
            catch (Exception ex)
            {
                connectionFromPostgres.Close();
            }

            return dialerState;


        }
    }
}
