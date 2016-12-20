using Raven.Client;
using Raven.Client.Document;

namespace ProjectionManager
{
    internal class ConnectionFactory
    {
        private readonly IDocumentStore _store;

        public ConnectionFactory(string database)
        {
            _store = new DocumentStore
            {
                Url = "http://localhost:8080/",
                DefaultDatabase = database
            };

            _store.Initialize();
        }

        public IDocumentSession Connect()
        {
            return _store.OpenSession();
        }
    }
}