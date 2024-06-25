using _3CXCallReporterLast.Models;
using _3CXCallReporterLast.Services;
using Microsoft.AspNetCore.Mvc;

namespace _3CXCallReporterLast.Controllers
{
    [ApiController]
    public class WallBoardController
    {
        [HttpPost("/GetCardInfo")]
        
        public ResponseCardInfoModel GetCardInfo()
        {
            return new CardInfoService().GetCardInfoService();

        }

    }
}
