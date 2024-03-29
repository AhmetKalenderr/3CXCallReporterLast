﻿using System;
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
            connectionFromPostgres.Open();

            try
            {
                string sql = $@"
                                INSERT INTO public.customers(
	                             ""customerName"", ""customerTc"", ""customerPhoneNumber"",""customerNote"",""customerPayment"")
	                            VALUES 
                ";

                foreach (var m in model)
                {
                    sql += $@"('{m.Name}','{m.TC}','{m.PhoneNumber}','{m.Note}','{m.Payment}'),";
                }
                sql = sql.Remove(sql.Length - 1);
                Console.WriteLine(sql);
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
        //Düzenlenecek..
        public CustomerForCSVModel GetDataByPhoneNumber(string phoneNumber)
        {
            NpgsqlConnection connectionFromPostgres = new NpgsqlConnection(GetConnectionStringClass.connFromPostgres);
            connectionFromPostgres.Open();
            CustomerForCSVModel model = new CustomerForCSVModel();
            phoneNumber = phoneNumber.Substring(phoneNumber.Length-9);
            try
            {
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

        public AgentModel GetAgentByAgentNumber(string agentNumber)
        {
            NpgsqlConnection connectionFromPostgres = new NpgsqlConnection(GetConnectionStringClass.connFromPostgres);
            connectionFromPostgres.Open();

            AgentModel agent = new AgentModel();
            try
            {
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
            connectionFromPostgres.Open();

            bool state = false;
            try
            {
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
            connectionFromPostgres.Open();

            bool state = false;
            try
            {
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

        public void CreateUserTableIfNotExists()
        {
            NpgsqlConnection connectionFromPostgres = new NpgsqlConnection(GetConnectionStringClass.connFromPostgres);
            connectionFromPostgres.Open();

            try
            {
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
