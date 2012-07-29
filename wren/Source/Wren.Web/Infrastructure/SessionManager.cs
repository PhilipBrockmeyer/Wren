using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Raven.Client;
using Wren.Services.Core;
using NHibernate;

namespace Wren.Web.Infrastructure
{
    public class SessionManager : ISessionManager
    {        
        public ISession GetSession()
        {
            return ((ISessionFactory)HttpContext.Current.Application["sessionFactory"]).OpenSession();
        }
    }
}