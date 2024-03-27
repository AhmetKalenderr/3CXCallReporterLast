using _3CXCallReporterLast.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace _3CXCallReporterLast.Controllers
{
    [ApiController]
    public class CustomerController
    {
        [HttpPost("/insertCustomer")]
        public string InsertData(List<CustomerForCSVModel> customers)
        {

            return string.Empty;
        }
    }
}
