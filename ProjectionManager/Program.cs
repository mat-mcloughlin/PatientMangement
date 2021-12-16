using System;
using System.Collections.Generic;
using EventStore.ClientAPI;
using ProjectionManager;

var eventStoreConnection = GetEventStoreConnection();

var connectionFactory = new ConnectionFactory("PatientManagement");

var projections = new List<IProjection>
{
    new WardViewProjection(connectionFactory),
    new PatientDemographicProjection(connectionFactory)
};

var projectionManager = new ProjectionManager.ProjectionManager(
    eventStoreConnection,
    connectionFactory,
    projections);

projectionManager.Start();

Console.WriteLine("Projection Manager Running");
Console.ReadLine();

IEventStoreConnection GetEventStoreConnection()
{
    const string connectionString = 
        "ConnectTo=tcp://localhost:1113;UseSslConnection=false;DefaultCredentials=admin:changeit";
    eventStoreConnection = EventStoreConnection.Create(connectionString);

    eventStoreConnection.ConnectAsync().Wait();
    return eventStoreConnection;
}