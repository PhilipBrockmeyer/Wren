using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Transport.DataObjects
{
    public class LogOnResultDto
    {
        public Boolean IsSuccessful { get; set; }
        public String[] Errors { get; set; }
        public String UserId { get; set; }
    }
}
