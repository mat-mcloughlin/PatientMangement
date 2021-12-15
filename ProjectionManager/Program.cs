using System;
using System.Collections.Generic;
using System.Threading;
using EventStore.Client;
using ProjectionManager;

var ct = new CancellationTokenSource().Token;

var eventStore = GetEventStore();

var connectionFactory = new ConnectionFactory("PatientManagement");
await connectionFactory.EnsureDatabaseExistsAsync(ct);

var projections = new List<IProjection>
{
    new WardViewProjection(connectionFactory),
    new PatientDemographicProjection(connectionFactory)
};

var projectionManager = new ProjectionManager.ProjectionManager(
    eventStore,
    connectionFactory,
    projections);

await projectionManager.StartAsync(ct);

Console.WriteLine("Projection Manager Running");
Console.ReadLine();

EventStoreClient GetEventStore()
{
    const string connectionString = 
        "esdb://localhost:2113?tls=false";
    return new EventStoreClient(EventStoreClientSettings.Create(connectionString));
}