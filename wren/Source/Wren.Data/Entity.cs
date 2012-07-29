using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wren.Data
{
    public abstract class Entity
    {
        public virtual String Id { get; set; }
    }
}