using Raven.Client;
using Raven.Client.Document;

namespace Explorer
{
    class ConnectionFactory
    {
        private IDocumentStore _store;

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