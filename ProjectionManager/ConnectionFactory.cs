using System;
using Raven.Client.Documents;
using Raven.Client.Documents.Operations;
using Raven.Client.Documents.Session;
using Raven.Client.Exceptions;
using Raven.Client.Exceptions.Database;
using Raven.Client.ServerWide;
using Raven.Client.ServerWide.Operations;

namespace ProjectionManager;

public class ConnectionFactory
{
    private readonly IDocumentStore _store;

    public ConnectionFactory(string database)
    {
        _store = new DocumentStore
        {
            Urls = new [] { "http://localhost:8080/" },
            Database = database
        };

        _store.Initialize();
        
        EnsureDatabaseExists(_store, database);
    }

    public IDocumentSession Connect()
    {
        return _store.OpenSession();
    }

    private static void EnsureDatabaseExists(IDocumentStore store, string? database = null, bool createDatabaseIfNotExists = true)
    {
        database ??= store.Database;

        if (string.IsNullOrWhiteSpace(database))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(database));

        try
        {
            store.Maintenance.ForDatabase(database).Send(new GetStatisticsOperation());
        }
        catch (DatabaseDoesNotExistException)
        {
            if (createDatabaseIfNotExists == false)
                throw;

            try
            {
                store.Maintenance.Server.Send(new CreateDatabaseOperation(new DatabaseRecord(database)));
            }
            catch (ConcurrencyException)
            {
                // The database was already created before calling CreateDatabaseOperation
            }

        }
    }
}