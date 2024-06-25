using System;
using System.Threading;
using _3CXCallReporterLast.Models;
using _3CXCallReporterLast.Services;
using Microsoft.AspNetCore.Mvc;
using TCX.Configuration;

namespace _3CXCallReporterLast.Controllers
{
    [ApiController]
    public class DialerController
    {
        [HttpGet("/CheckPaymentAndOpenOrCloseDialer")]
        public DialerOpenOrCloseModel CheckPaymentAndOpenOrCloseDialer(Boolean state)
        {
            return new DialerService().CheckPaymentAndOpenOrCloseDialer(state);
        }

        [HttpGet("/CheckDialerState")]
        public Boolean CheckDialerState()
        {
            return new DialerService().CheckDialerState();
        }

        [HttpGet("/ChangePaymentState")]
        public string ChangePaymentState(Boolean state)
        {
            return new DialerService().ChangeState(state);
        }
    }
}
