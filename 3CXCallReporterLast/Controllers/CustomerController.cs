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
        public CsvInsertDataResponseModel InsertData([FromBody]List<CustomerForCSVModel> customers)
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

        [HttpPost("/deleteAllData")]
        public bool DeleteAllData()
        {
            return customData.DeleteAllData();
        }

        [HttpPost("/deleteTodayInsertedDataCustomerData")] 
        public bool DeleteTodayInsertedDataCustomerData()
        {
            return customData.DeleteTodayInsertedDataCustomerData();
        }

        [HttpPost("/getCountCustomerData")]
        public int GetCountCustomerData()
        {
            return customData.GetCountCustomerData();
        }

        [HttpPost("/deleteDataByGuid")]
        public bool deleteDataByGuid([FromBody]RequestIdModel id)
        {
            return customData.DeleteDataByGuid(id.Id);
        }

        [HttpPost("/deleteDataById")]
        public bool DeleteDataById([FromBody] RequestIdModel id)
        {
            return customData.DeleteDataById(id.Id);
        }

        [HttpPost("/getGroupCsv")]
        public List<GroupCsvModel> GetGroupCsv()
        {
            return customData.GetGroupCsv();
        }

        [HttpPost("/getGroupCsvDetails")]
        public List<CustomerForCSVModel> GetGroupCsvDetails([FromBody]RequestIdModel id)
        {
            return customData.GetGroupCsvDetails(id.Id);
        }


    }
}
