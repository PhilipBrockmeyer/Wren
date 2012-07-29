using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Data.Entities
{
    public class TosecInfo : Entity
    {
        public virtual String Name { get; set; }
        public virtual String Publisher { get; set; }
        public virtual String Year { get; set; }
        public virtual String Country { get; set; }
        public virtual String System { get; set; }
    }
}
