using _3CXCallReporterLast.Models.LoginModel;
using _3CXCallReporterLast.Repository;
using Microsoft.AspNetCore.Mvc;
using TCX.Configuration;

namespace _3CXCallReporter.Controllers
{
    [ApiController]
    public class LoginController : Controller
    {
        [HttpPost("/login")]
        public LoginResponseModel Login([FromBody] LoginRequestModel login)
        {

            if (login.Name == "admin")
            {
                string password = MasterDatabaseRepository.GetDatabase("select configuration_webadmin_pass from customers ");
                if (login.Password == password)
                {
                    return new LoginResponseModel
                    {
                        LoginStatus = true,
                        Role = "admin"
                    };
                }
                else
                {
                    Response.StatusCode = 401;
                    return new LoginResponseModel
                    {
                        LoginStatus = false,
                        Role = string.Empty
                    };
                }
            }
            else
            {
                if (login.Password == PhoneSystem.Root.GetDNByNumber(login.Name).GetPropertyValue("SERVICES_ACCESS_PASSWORD"))
                {
                    return new LoginResponseModel
                    {
                        LoginStatus = true,
                        Role = "agent"
                    };
                }
                else
                {
                    Response.StatusCode = 401;
                    return new LoginResponseModel
                    {
                        LoginStatus = false,
                        Role = string.Empty
                    };
                }
            }


        }
    }
}
