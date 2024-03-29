using _3CXCallReporterLast.Models.LoginModel;
using _3CXCallReporterLast.Repository;
using _3CXCallReporterLast.Services;
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
                CustomDatabaseRepository customDatabaseRepository = new CustomDatabaseRepository();
                if (PhoneSystem.Root.GetDNByNumber(login.Name) ==null)
                {
                    Response.StatusCode = 401;
                    return new LoginResponseModel
                    {
                        LoginStatus = false,
                        Role = string.Empty
                    };
                }
                else
                {
                    if (customDatabaseRepository.GetAgentByAgentNumber(login.Name).AgentPassword ==login.Password)
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
}
