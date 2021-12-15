using System;
using System.Threading;
using System.Threading.Tasks;
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
        
    }

    public IAsyncDocumentSession  Connect()
    {
        return _store.OpenAsyncSession();
    }

    public async Task EnsureDatabaseExistsAsync(CancellationToken ct)
    {
        var database = _store.Database;

        if (string.IsNullOrWhiteSpace(database))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(database));

        try
        {
            await _store.Maintenance.ForDatabase(database).SendAsync(new GetStatisticsOperation(), ct);
        }
        catch (DatabaseDoesNotExistException)
        {
            try
            {
                await _store.Maintenance.Server.SendAsync(new CreateDatabaseOperation(new DatabaseRecord(database)), ct);
            }
            catch (ConcurrencyException)
            {
                // The database was already created before calling CreateDatabaseOperation
            }
        }
    }
}