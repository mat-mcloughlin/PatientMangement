using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using PatientManagement.AdmissionDischargeTransfer.Commands;
using PatientManagement.Framework;
using PatientManagement.Framework.Commands;

namespace SeedGenerator
{
    internal class Program
    {
        static readonly Random Random = new Random();

        public static async Task Main(string[] args)
        {
            var listOfPatients = Patient.Generate(400);

            var dispatcher = await SetupDispatcher();

            await AdmitPatients(listOfPatients, dispatcher);
            await TransferPatients(listOfPatients, dispatcher);
            await DischargeAllPatients(listOfPatients, dispatcher);

            listOfPatients = Patient.Generate(20);

            await AdmitPatients(listOfPatients, dispatcher);
            await TransferPatients(listOfPatients, dispatcher);
            await DischargePatients(listOfPatients, dispatcher);
        }

        private static async Task DischargePatients(IEnumerable<Patient> listOfPatients, Dispatcher dispatcher)
        {
            foreach (var patient in listOfPatients)
            {
                var discharge = Random.Next(0, 5) == 0;
                if (discharge)
                {
                    await dispatcher.Dispatch(new DischargePatient(patient.Id));
                }

                Console.WriteLine("Discharging: " + patient.Name);
            }
        }

        public static async Task DischargeAllPatients(IEnumerable<Patient> listOfPatients, Dispatcher dispatcher)
        {
            foreach (var patient in listOfPatients)
            {
                await dispatcher.Dispatch(new DischargePatient(patient.Id));

                Console.WriteLine("Discharging: " + patient.Name);
            }
        }

        public static async Task TransferPatients(List<Patient> listOfPatients, Dispatcher dispatcher)
        {
            for (var i = 0; i < 10; i++)
            {
                foreach (var patient in listOfPatients)
                {
                    var transfer = Random.Next(0, 5) == 0;
                    if (transfer)
                    {
                        await dispatcher.Dispatch(new TransferPatient(patient.Id, Ward.Get()));
                        Console.WriteLine("Transfering: " + patient.Name);
                    }
                }
            }
        }

        public static async Task AdmitPatients(IEnumerable<Patient> listOfPatients, Dispatcher dispatcher)
        {
            foreach (var patient in listOfPatients)
            {
                await dispatcher.Dispatch(new AdmitPatient(
                    patient.Id,
                    patient.Name,
                    patient.Age,
                    DateTime.UtcNow,
                    Ward.Get()));
                Console.WriteLine("Admitting: " + patient.Name);
            }
        }

        private static async Task<Dispatcher> SetupDispatcher()
        {
            var connectionSettings = ConnectionSettings.Create()
                .DisableTls();

            var eventStoreConnection = EventStoreConnection.Create(
                connectionSettings,
                new IPEndPoint(IPAddress.Loopback, 1113));

            await eventStoreConnection.ConnectAsync();
            var repository = new AggregateRepository(eventStoreConnection);

            var commandHandlerMap = new CommandHandlerMap(new Handlers(repository));

            return new Dispatcher(commandHandlerMap);

        }
    }
}
