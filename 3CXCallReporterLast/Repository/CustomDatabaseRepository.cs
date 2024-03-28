using System;
using System.Collections.Generic;
using _3CXCallReporterLast.Helpers;
using _3CXCallReporterLast.Models;
using Npgsql;

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
	                             ""customerName"", ""customerTc"", ""customerPhoneNumber"")
	                            VALUES 
                ";

                foreach (var m in model)
                {
                    sql += $@"({m.Name},{m.TC},{m.PhoneNumber})";
                }
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
        public AgentConnection GetDataByPhoneNumber(string phoneNumber)
        {
            NpgsqlConnection connectionFromPostgres = new NpgsqlConnection(GetConnectionStringClass.connFromPostgres);
            connectionFromPostgres.Open();
            AgentConnection agent = new AgentConnection();
            try
            {
                string sql = $@"SELECT id, ""customerName"", ""customerTc"", ""customerPhoneNumber"", ""customerNote"", ""customerPayment""
	            FROM public.customers where ""customerPhoneNumber"" ilike '%{phoneNumber}%'";



            }
            catch (Exception ex)
            {
                return agent;
            }

            return agent;
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
