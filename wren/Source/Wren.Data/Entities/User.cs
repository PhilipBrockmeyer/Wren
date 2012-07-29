using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wren.Data.Entities
{
    public class User : Entity
    {
        public virtual String Username { get; set; }
        public virtual String LoweredUsername { get; set; }
        public virtual String Password { get; set; }
        public virtual String EmailAddress { get; set; }
    }
}