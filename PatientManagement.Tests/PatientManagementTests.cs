using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventStore.Client;
using PatientManagement.AdmissionDischargeTransfer.Commands;
using PatientManagement.Framework;
using PatientManagement.Framework.Commands;
using Polly;
using Polly.Retry;
using ProjectionManager;
using Xunit;

namespace PatientManagement.Tests;

public class PatientManagementTests
{
    private readonly AsyncRetryPolicy retryPolicy =
        Policy.Handle<Exception>().WaitAndRetryAsync(5, i => TimeSpan.FromSeconds(i));
    
    [Fact]
    public async Task EndToEndTest()
    {
        var ct = new CancellationTokenSource().Token;
        var eventStore = GetEventStore();
        var dispatcher = SetupDispatcher(eventStore);
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
        
        var patientId = Guid.NewGuid();

        var admitPatient = new AdmitPatient(patientId, "Tony Ferguson", 32, DateTime.UtcNow, 10);
        await dispatcher.Dispatch(admitPatient, ct);

        await retryPolicy.ExecuteAsync(async token =>
        {
            using var session = connectionFactory.Connect();
            var patient = await session.LoadAsync<Patient>(patientId.ToString(), token);
            
            Assert.NotNull(patient);
            Assert.Equal(patientId.ToString(), patient.Id);
            Assert.Equal(admitPatient.PatientName, patient.PatientName);
            Assert.Equal(admitPatient.AgeInYears, patient.AgeInYears);
            Assert.Equal(admitPatient.WardNumber, patient.WardNumber);
        }, ct);

        var transferPatientOne = new TransferPatient(patientId, 76);
        await dispatcher.Dispatch(transferPatientOne, ct);
        
        await retryPolicy.ExecuteAsync(async token =>
        {
            using var session = connectionFactory.Connect();
            var patient = await session.LoadAsync<Patient>(patientId.ToString(), token);
            
            Assert.NotNull(patient);
            Assert.Equal(transferPatientOne.WardNumber, patient.WardNumber);
        }, ct);

        var transferPatientTwo = new TransferPatient(patientId, 34);
        await dispatcher.Dispatch(transferPatientTwo, ct);
        
        await retryPolicy.ExecuteAsync(async token =>
        {
            using var session = connectionFactory.Connect();
            var patient = await session.LoadAsync<Patient>(patientId.ToString(), token);
            
            Assert.NotNull(patient);
            Assert.Equal(transferPatientTwo.WardNumber, patient.WardNumber);
        }, ct);

        var dischargePatient = new DischargePatient(patientId);
        await dispatcher.Dispatch(dischargePatient, ct);
        
        await retryPolicy.ExecuteAsync(async token =>
        {
            using var session = connectionFactory.Connect();
            var patient = await session.LoadAsync<Patient>(patientId.ToString(), token);
            
            Assert.Null(patient);
        }, ct);
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
}