﻿using System;
using System.Collections.Generic;
using System.Net;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
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
    ConnectionSettings settings = ConnectionSettings.Create()
        .SetDefaultUserCredentials(new UserCredentials("admin", "changeit"));

    var eventStoreConnection = EventStoreConnection.Create(
        settings,
        new IPEndPoint(IPAddress.Loopback, 1113));

    eventStoreConnection.ConnectAsync().Wait();
    return eventStoreConnection;
}