using _3CXCallReporterLast.Helpers;
using Npgsql;
using System.Data;

namespace _3CXCallReporterLast.Repository
{
    public class MasterDatabaseRepository
    {
        public static string GetDatabase(string query)
        {
            NpgsqlConnection connectionFromMaster = new NpgsqlConnection(GetConnectionStringClass.connFromMaster);

            connectionFromMaster.Open();

            NpgsqlDataAdapter da = new NpgsqlDataAdapter(query, connectionFromMaster);
            DataSet ds = new DataSet();
            da.Fill(ds);

            string jsonString = "";
            jsonString += "";

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                jsonString += "";

                foreach (DataColumn column in ds.Tables[0].Columns)
                {
                    jsonString += row[column];
                }

                jsonString += "";
            }


            jsonString += "";

            connectionFromMaster.Close();


            return jsonString;


        }

    }
}
