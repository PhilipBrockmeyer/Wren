using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Transport;
using Wren.Transport.DataObjects;

namespace Wren.Transport.ServiceContracts
{
    public interface IAccountService
    {
        RegistrationValidationResultDto IsRegistrationValid(String username, String password, String email);
        void RegisterUser(String id, String username, String password, String email);
    }
}
