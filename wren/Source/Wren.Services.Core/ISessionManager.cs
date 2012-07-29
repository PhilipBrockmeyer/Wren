using System;
using Raven.Client;
using NHibernate;

namespace Wren.Services.Core
{
    public interface ISessionManager
    {
        ISession GetSession();
    }
}
