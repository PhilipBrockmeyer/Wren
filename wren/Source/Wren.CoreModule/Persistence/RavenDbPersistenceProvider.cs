using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client;
using System.IO;
using Raven.Client.Document;
using Wren.Core.Achievements;
using Raven.Client.Indexes;

namespace Wren.Core.Persistence
{
    public class RavenDbPersistenceProvider : IPersistenceProvider
    {
        static IDocumentStore _documentStore;

        static RavenDbPersistenceProvider()
        {
            var dataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Wren", "Data");

            if (!System.IO.Directory.Exists(dataPath))
                System.IO.Directory.CreateDirectory(dataPath);

            try
            {
                _documentStore = new DocumentStore() { DataDirectory = dataPath };
                _documentStore.Initialize();

                BuildIndexes();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("There was an error initializing the embedded database.", ex);
            }
        }

        private static void BuildIndexes()
        {
            if (_documentStore.DatabaseCommands.GetIndex("AchievementDefinitions/RomMd5") == null)
            {
                _documentStore.DatabaseCommands.PutIndex<AchievementDefinition, AchievementDefinition>
                    ("AchievementDefinitions/RomMd5", new IndexDefinition<AchievementDefinition, AchievementDefinition>()
                    {
                        Map = docs => from doc in docs
                                      select new
                                      {
                                          RomMd5 = doc.RomMd5
                                      }
                    });
            }

            if (_documentStore.DatabaseCommands.GetIndex("AchievementState/NotUploaded") == null)
            {
                _documentStore.DatabaseCommands.PutIndex<AchievementState, AchievementState>
                    ("AchievementState/NotUploaded", new IndexDefinition<AchievementState, AchievementState>()
                    {
                        Map = docs => from doc in docs
                                      where !doc.IsUploaded
                                      select new
                                      {
                                          UserId = doc.UserId                             
                                      }
                    });
            }

            if (_documentStore.DatabaseCommands.GetIndex("AchievementState/GameAndUser") == null)
            {
                _documentStore.DatabaseCommands.PutIndex<AchievementState, AchievementState>
                    ("AchievementState/GameAndUser", new IndexDefinition<AchievementState, AchievementState>()
                    {
                        Map = docs => from doc in docs
                                      select new
                                      {
                                          GameId = doc.RomMd5, UserId = doc.UserId
                                      }
                    });
            }
        }

        public T Load<T>(String itemName)
        {
            using (var session = _documentStore.OpenSession())
            {
                return session.Load<T>(itemName);
            }
        }

        public void Save(String itemName, Object obj)
        {
            using (var session = _documentStore.OpenSession())
            {
                session.Store(obj);
                session.SaveChanges();
            }
        }

        public IEnumerable<T> LoadMany<T>(String query)
        {
            String index = query.Split(' ')[0];
            String whereClause = query.Split(' ')[1];

            using (var session = _documentStore.OpenSession())
            {
                var result = session.LuceneQuery<T>(index)
                    .WaitForNonStaleResults()
                    .Where(whereClause);
                return result;
            }
        }
    }
}
