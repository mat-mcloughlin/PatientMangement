using System;
using System.Collections.Generic;
using System.Threading;
using EventStore.Client;
using EventStore.ClientAPI;
using PatientManagement.AdmissionDischargeTransfer.Commands;
using PatientManagement.Framework;
using PatientManagement.Framework.Commands;
using ProjectionManager;

var ct = new CancellationTokenSource().Token;

var eventStoreConnection = GetEventStoreConnection();
var eventStore = GetEventStore();
var dispatcher = SetupDispatcher(eventStore);
var connectionFactory = new ConnectionFactory("PatientManagement");

var patientId = Guid.NewGuid();

var admitPatient = new AdmitPatient(patientId, "Tony Ferguson", 32, DateTime.UtcNow, 10);
await dispatcher.Dispatch(admitPatient, ct);

var transferPatientOne = new TransferPatient(patientId, 76);
await dispatcher.Dispatch(transferPatientOne, ct);

var transferPatientTwo = new TransferPatient(patientId, 34);
await dispatcher.Dispatch(transferPatientTwo, ct);

var dischargePatient = new DischargePatient(patientId);
await dispatcher.Dispatch(dischargePatient, ct);

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
    var eventStoreConnection = EventStoreConnection.Create(connectionString);

    eventStoreConnection.ConnectAsync().Wait();
    return eventStoreConnection;
}

EventStoreClient GetEventStore()
{
    const string connectionString = 
        "esdb://localhost:2113?tls=false";
    return new EventStoreClient(EventStoreClientSettings.Create(connectionString));
}

Dispatcher SetupDispatcher(EventStoreClient eventStore)
{
    var repository = new AggregateRepository(eventStore);

    var commandHandlerMap = new CommandHandlerMap(new Handlers(repository));

    return new Dispatcher(commandHandlerMap);
}