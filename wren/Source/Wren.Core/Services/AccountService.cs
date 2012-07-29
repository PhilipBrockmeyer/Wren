using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Net;
using Newtonsoft.Json;
using System.Collections.Specialized;
using Wren.Transport.ServiceContracts;
using Wren.Transport.DataObjects;

namespace Wren.Core.Services
{
    public class AccountService : Service, IAccountService
    {
        private class RegistrationValidationParameters : IServiceParameters
        {
            public String Username { get; set; }
            public String Password { get; set; }
            public String Email { get; set; }
        }

        public RegistrationValidationResultDto IsRegistrationValid(String username, String password, String email)
        {
            var parameters = new RegistrationValidationParameters() { Username = username, Password=password, Email=email };
            return base.WebPost<RegistrationValidationResultDto>(parameters, "Account/IsRegistrationValid");            
        }

        private class RegistrationParameters : IServiceParameters
        {
            public String UserId { get; set; }
            public String Username { get; set; }
            public String Password { get; set; }
            public String Email { get; set; }
        }

        public void RegisterUser(String id, String username, String password, String email)
        {
            var parameters = new RegistrationParameters() { UserId = id, Username = username, Password = password, Email = email };
            if (!base.WebPost<Boolean>(parameters, "Account/Register"))
                throw new ApplicationException("There was an error registering the user.  Application is shutting down.");
        }

        private class LogOnParameters : IServiceParameters
        {
            public String Username { get; set; }
            public String Password { get; set; }
        }

        public LogOnResultDto LogOn(String username, String password)
        {
            var parameters = new LogOnParameters() { Username = username, Password = password };
            return base.WebPost<LogOnResultDto>(parameters, "Account/LogOn");
        }
    }
}
