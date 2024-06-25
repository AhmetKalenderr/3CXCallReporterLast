using _3CXCallReporterLast.Models;
using _3CXCallReporterLast.Repository;
using System;

namespace _3CXCallReporterLast.Services
{
    public class DialerService
    {
        CustomDatabaseRepository customData = new CustomDatabaseRepository();
        public DialerOpenOrCloseModel CheckPaymentAndOpenOrCloseDialer(bool state)
        {
            return customData.CheckPaymentAndOpenOrCloseDialer(state);
        }

        public string ChangeState(bool state)
        {
            return customData.ChangeState(state);
        }

        public bool CheckDialerState()
        {
            return  customData.CheckDialerState();
        }
    }
}
