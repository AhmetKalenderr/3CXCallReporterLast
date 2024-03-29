using _3CXCallReporterLast.Models;
using _3CXCallReporterLast.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace _3CXCallReporterLast.Controllers
{
    [ApiController]
    public class CustomerController
    {
        CustomDatabaseRepository customData = new CustomDatabaseRepository();

        [HttpPost("/insertCustomer")]
        public string InsertData([FromBody]List<CustomerForCSVModel> customers)
        {
            return customData.InsertData(customers);
        }

        [HttpPost("/updateNote")]
        public bool UpdateNote([FromBody] UpdateNoteCustomer customerNote)
        {
            return customData.UpdateNote(customerNote);
        }

        [HttpPost("/deleteLastInsertedData")]
        public bool DeleteLastInsertedData()
        {
            return customData.DeleteLastInsertedData();
        }


    }
}
