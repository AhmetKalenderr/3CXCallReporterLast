using System.Collections.Generic;
using System;

namespace _3CXCallReporterLast.Helpers
{
    public class ConnectionString
    {
        public static Dictionary<string, Dictionary<string, string>> iniContent =
           new Dictionary<string, Dictionary<string, string>>(
           StringComparer.InvariantCultureIgnoreCase);


        public string connFromMaster { get; set; } = "server=" + iniContent["CfgServerProfile"]["DBHost"] + "; port=" + iniContent["CfgServerProfile"]["DBPort"] + "; Database=masterprofiles" + "; user ID=" + iniContent["DbAdminREADONLY"]["User"] + "; password=" + iniContent["DbAdminREADONLY"]["Password"] + ";" + "CommandTimeout=300;";

        public static string connFromSingle { get; set; } = "server=" + iniContent["CfgServerProfile"]["DBHost"] + "; port=" + iniContent["CfgServerProfile"]["DBPort"] + "; Database=" + iniContent["QMDatabase"]["DBName"] + "; user ID=" + iniContent["DbAdminREADONLY"]["User"] + "; password=" + iniContent["DbAdminREADONLY"]["Password"] + ";" + "CommandTimeout=300;";

        public  string connFromPostgres { get; set; } = "server=" + iniContent["CfgServerProfile"]["DBHost"] + "; port=" + iniContent["CfgServerProfile"]["DBPort"] + "; Database=postgres" + "; user ID=" + iniContent["DbAdminREADONLY"]["User"] + "; password=" + iniContent["DbAdminREADONLY"]["Password"] + "; CommandTimeout=300";

    }
}
