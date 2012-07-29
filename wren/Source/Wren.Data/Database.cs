using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using FluentNHibernate.Cfg;
using FluentNHibernate.Automapping;
using Wren.Data.Entities;
using NHibernate.Cfg;
using System.IO;
using NHibernate.Tool.hbm2ddl;

namespace Wren.Data
{
    public static class Database
    {
        public static ISessionFactory Initialize()
        {
            StoreConfiguration cfg = new StoreConfiguration();

            return Fluently.Configure()
                .Database(FluentNHibernate.Cfg.Db.SQLiteConfiguration.Standard.UsingFile(@"C:\WData\data.sqlite"))
                .Mappings(m =>
                    m.AutoMappings.Add(
                        AutoMap.AssemblyOf<Entity>(cfg))
                )
                .ExposeConfiguration(ExportSchema)
                .BuildSessionFactory();
        }

        public static void ExportSchema(Configuration cfg)
        {
            // delete the existing db on each run
            if (File.Exists(@"C:\WData\data.sqlite"))
                return;

            // this NHibernate tool takes a configuration (with mapping info in)
            // and exports a database schema from it
            new SchemaExport(cfg)
              .Create(false, true);
        }
    }

    public class StoreConfiguration : DefaultAutomappingConfiguration
    {
        public override Boolean ShouldMap(Type type)
        {
            return type.Namespace == "Wren.Data.Entities";
        }
    }
}
