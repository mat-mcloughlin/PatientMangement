using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using PatientManagement.AdmissionDischargeTransfer.Commands;
using PatientManagement.Framework;
using PatientManagement.Framework.Commands;
using ProjectionManager;

var eventStoreConnection = GetEventStoreConnection();
var dispatcher = SetupDispatcher(eventStoreConnection);
var connectionFactory = new ConnectionFactory("PatientManagement");

var patientId = Guid.NewGuid();

var admitPatient = new AdmitPatient(patientId, "Tony Ferguson", 32, DateTime.UtcNow, 10);
await dispatcher.Dispatch(admitPatient);

var transferPatientOne = new TransferPatient(patientId, 76);
await dispatcher.Dispatch(transferPatientOne);

var transferPatientTwo = new TransferPatient(patientId, 34);
await dispatcher.Dispatch(transferPatientTwo);

var dischargePatient = new DischargePatient(patientId);
await dispatcher.Dispatch(dischargePatient);

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

Dispatcher SetupDispatcher(IEventStoreConnection eventStoreConnection)
{
    var repository = new AggregateRepository(eventStoreConnection);

    var commandHandlerMap = new CommandHandlerMap(new Handlers(repository));

    return new Dispatcher(commandHandlerMap);
}