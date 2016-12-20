using System;
using System.Net;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using PatientManagement.AdmissionDischargeTransfer.Commands;
using PatientManagement.Framework;
using PatientManagement.Framework.Commands;

namespace TestConsole
{
    class Program
    {
        static void Main()
        {
            AsyncMain().GetAwaiter().GetResult();
        }

        static async Task AsyncMain()
        {
            var dispatcher = await SetupDispatcher();

            var patientId = Guid.NewGuid();

            var admitPatient = new AdmitPatient(patientId, "Tony Ferguson", 32, DateTime.UtcNow, 10);
            await dispatcher.Dispatch(admitPatient);

            var transferPatientOne = new TransferPatient(patientId, 76);
            await dispatcher.Dispatch(transferPatientOne);

            var transferPatientTwo = new TransferPatient(patientId, 34);
            await dispatcher.Dispatch(transferPatientTwo);

            var dischargePatient = new DischargePatient(patientId);
            await dispatcher.Dispatch(dischargePatient);


            Console.ReadLine();
        }

        private static async Task<Dispatcher> SetupDispatcher()
        {
            var eventStoreConnection = EventStoreConnection.Create(
                ConnectionSettings.Default,
                new IPEndPoint(IPAddress.Loopback, 1113));

            await eventStoreConnection.ConnectAsync();
            var repository = new AggregateRepository(eventStoreConnection);

            var commandHandlerMap = new CommandHandlerMap(new Handlers(repository));

            return new Dispatcher(commandHandlerMap);

        }
    }
}
