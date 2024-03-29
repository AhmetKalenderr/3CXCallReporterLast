using _3CXCallReporterLast.Models;
using _3CXCallReporterLast.Models.LoginModel;
using _3CXCallReporterLast.Repository;
using TCX.Configuration;

namespace _3CXCallReporterLast.Services
{
    public class AgentService
    {
        public RegisterResponseModel RegisterAgent(AgentModel model)
        {
            CustomDatabaseRepository customRepo = new CustomDatabaseRepository();
            RegisterResponseModel responseModel = new RegisterResponseModel();
            var agent = customRepo.GetAgentByAgentNumber(model.AgentNumber);

            if (PhoneSystem.Root.GetDNByNumber(model.AgentNumber) == null)
            {
                responseModel.RegisterMessage = "Hatalı Agent Numarası Girdiniz";
                responseModel.RegisterState = false;
            }else if(agent.AgentNumber != null)
            {
                responseModel.RegisterMessage = "Zaten Kayıtlı bir Agent numarası ile kayıt oluşturmaya çalışıyorsunuz.";
                responseModel.RegisterState = false;
            }else 
            {
                if (customRepo.RegisterAgent(model))
                {
                    responseModel.RegisterMessage = "Kayıt Başarılı";
                    responseModel.RegisterState = true;
                }else
                {
                    responseModel.RegisterMessage = "Kayıt oluşturulurken beklenmedik bir hata oluştu";
                    responseModel.RegisterState = false;
                }
            }
            

            return responseModel;

        }
    }
}
