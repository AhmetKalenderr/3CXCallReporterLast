using System.Collections.Generic;
using System;

namespace _3CXCallReporterLast.Helpers
{
    public class GetConnectionStringClass
    {
        public static Dictionary<string, Dictionary<string, string>> iniContent =
           new Dictionary<string, Dictionary<string, string>>(
           StringComparer.InvariantCultureIgnoreCase);


        public static string connFromMaster { get; set; } = "server=127.0.0.1; port=5480; Database=masterprofiles; user ID=phonesystem; password=o2EBnaSrm3hI6;" + "CommandTimeout=300;";

        public static string connFromSingle { get; set; } = "server=127.0.0.1; port=5480; Database=database_single; user ID=phonesystem; password=o2EBnaSrm3hI6;" + "CommandTimeout=300;";

        public static string connFromPostgres { get; set; } = "server=127.0.0.1; port=5432; Database=postgres; user ID=postgres; password=1234;" + "CommandTimeout=300;";

    }
}
