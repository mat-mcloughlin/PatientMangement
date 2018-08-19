using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using Raven.Client.ServerWide;
using Raven.Client.ServerWide.Operations;

namespace ProjectionManager
{
    public class ConnectionFactory
    {
        private readonly IDocumentStore _store;

        public ConnectionFactory(string database)
        {
            _store = new DocumentStore
            {
                Urls = new[] { "http://localhost:8080/" },
                Database = database
            };
            _store.Initialize();

            try
            {
                _store.Maintenance.Server.Send(new CreateDatabaseOperation(new DatabaseRecord(database)));
            }
            catch (Raven.Client.Exceptions.ConcurrencyException e) when (e.Message == $"Database '{database}' already exists!")
            {
                // Database already created, swallow the exception              
            }
        }

        public IDocumentSession Connect()
        {
            return _store.OpenSession();
        }
    }
}