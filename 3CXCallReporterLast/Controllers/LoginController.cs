using System;
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
                    Extension Agent = (Extension)PhoneSystem.Root.GetDNByNumber(login.Name);
                    foreach (var item in Agent.Properties)
                    {
                        Console.WriteLine(item.Name +"-"+item.Value);
                    }
                    if (login.Password == "0000")
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
