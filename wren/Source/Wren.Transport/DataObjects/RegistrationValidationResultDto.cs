using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Transport.DataObjects
{
    public class RegistrationValidationResultDto
    {
        public Boolean IsValid { get; set; }
        public String[] Errors { get; set; }
        public String UserId { get; set; }
    }
}
