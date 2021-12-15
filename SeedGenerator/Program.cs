using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventStore.Client;
using PatientManagement.AdmissionDischargeTransfer.Commands;
using PatientManagement.Framework;
using PatientManagement.Framework.Commands;
using SeedGenerator;

var random = new Random();

var listOfPatients = Patient.Generate(400);

var dispatcher = SetupDispatcher();

await AdmitPatients(listOfPatients, dispatcher);
await TransferPatients(listOfPatients, dispatcher);
await DischargeAllPatients(listOfPatients, dispatcher);

listOfPatients = Patient.Generate(20);

await AdmitPatients(listOfPatients, dispatcher);
await TransferPatients(listOfPatients, dispatcher);
await DischargePatients(listOfPatients, dispatcher);


async Task DischargePatients(IEnumerable<Patient> listOfPatients, Dispatcher dispatcher)
{
    foreach (var patient in listOfPatients)
    {
        var discharge = random.Next(0, 5) == 0;
        if (discharge)
        {
            await dispatcher.Dispatch(new DischargePatient(patient.Id));
        }

        Console.WriteLine($"Discharging: {patient.Name} ({patient.Id})");
    }
}

async Task DischargeAllPatients(IEnumerable<Patient> listOfPatients, Dispatcher dispatcher)
{
    foreach (var patient in listOfPatients)
    {
        await dispatcher.Dispatch(new DischargePatient(patient.Id));

        Console.WriteLine($"Discharging: {patient.Name} ({patient.Id})");
    }
}

async Task TransferPatients(List<Patient> listOfPatients, Dispatcher dispatcher)
{
    for (var i = 0; i < 10; i++)
    {
        foreach (var patient in listOfPatients)
        {
            var transfer = random.Next(0, 5) == 0;
            if (!transfer) continue;
            
            await dispatcher.Dispatch(new TransferPatient(patient.Id, Ward.Get()));
            Console.WriteLine($"Transferring: {patient.Name} ({patient.Id})");
        }
    }
}

async Task AdmitPatients(IEnumerable<Patient> listOfPatients, Dispatcher dispatcher)
{
    foreach (var patient in listOfPatients)
    {
        await dispatcher.Dispatch(new AdmitPatient(
            patient.Id,
            patient.Name,
            patient.Age,
            DateTime.UtcNow,
            Ward.Get()));
        Console.WriteLine($"Admitting: {patient.Name} ({patient.Id})");
    }
}

Dispatcher SetupDispatcher()
{
    const string connectionString = 
        "esdb://localhost:2113?tls=false";
    var eventStore = new EventStoreClient(EventStoreClientSettings.Create(connectionString));

    var repository = new AggregateRepository(eventStore);

    var commandHandlerMap = new CommandHandlerMap(new Handlers(repository));

    return new Dispatcher(commandHandlerMap);
}