using System;
using System.Collections.Generic;
using System.Net;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using MongoDB.Driver;

namespace ProjectionManager
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var eventStoreConnection = GetEventStoreConnection();

            var mongoClient = new MongoClient("mongodb://localhost");
            var mongoDatabase = mongoClient.GetDatabase("PatientManagement");

            var projections = new List<IProjection>
            {
                new WardViewProjection(mongoDatabase),
                new PatientDemographicProjection(mongoDatabase)
            };

            var projectionManager = new ProjectionManager(
                eventStoreConnection,
                mongoDatabase,
                projections);

            projectionManager.Start();

            Console.WriteLine("Projection Manager Running");
            Console.ReadLine();
        }

        static IEventStoreConnection GetEventStoreConnection()
        {
            ConnectionSettings settings = ConnectionSettings.Create()
                .SetDefaultUserCredentials(new UserCredentials("admin", "changeit"))
                .DisableTls();

            var eventStoreConnection = EventStoreConnection.Create(
                settings,
                new IPEndPoint(IPAddress.Loopback, 1113));

            eventStoreConnection.ConnectAsync().Wait();
            return eventStoreConnection;
        }
    }
}
