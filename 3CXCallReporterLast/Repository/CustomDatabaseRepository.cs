using System;
using _3CXCallReporterLast.Helpers;
using _3CXCallReporterLast.Models;
using Npgsql;

namespace _3CXCallReporterLast.Repository
{
    public class CustomDatabaseRepository
    {
        public string InsertData(CustomerForCSVModel model)
        {
            NpgsqlConnection connectionFromPostgres = new NpgsqlConnection(GetConnectionStringClass.connFromPostgres);
            connectionFromPostgres.Open();

            try
            {
                string sql = "";
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

        public string GetDataByPhoneNumber(string phoneNumber)
        {
            NpgsqlConnection connectionFromPostgres = new NpgsqlConnection(GetConnectionStringClass.connFromPostgres);
            connectionFromPostgres.Open();
            try
            {
                string sql = "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return string.Empty;
        }

        public void CreateUserTableIfNotExists()
        {
            NpgsqlConnection connectionFromPostgres = new NpgsqlConnection(GetConnectionStringClass.connFromPostgres);
            connectionFromPostgres.Open();

            try
            {
                string sql = "";
                NpgsqlCommand command = new NpgsqlCommand(sql,connectionFromPostgres);

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
