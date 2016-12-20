using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using EventStore.ClientAPI;
using Explorer.Web;
using Newtonsoft.Json;

namespace Explorer
{
    class Repository
    {
        private readonly ConnectionFactory _connectionFactory;

        public Repository(ConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        void ProjectionView()
        {
            using (var session = _connectionFactory.Connect())
            {
               
            }
        }

        public List<Ward> GetWardView()
        {
            using (var session = _connectionFactory.Connect())
            {
               // var collection = db.GetCollection<Patient>("wardView");

                return session.Query<Patient>()
                    .ToList()
                    .GroupBy(p => p.WardNumber)
                    .Select(g => new Ward {WardNumber = g.Key, Patients = g.ToList()})
                    .ToList();
            }
        }

        public Patient GetPatientView(string id)
        {
            using (var session = _connectionFactory.Connect())
            {
                //var collection = db.GetCollection<Patient>("wardView");

                //var patient = collection.FindById("fc003184-33b2-4874-803c-92b8e72cf030");
                return session.Load<Patient>(id);
            }
        }

        public List<Range> WardDemographicView()
        {
            using (var session = _connectionFactory.Connect())
            {
                //var collection = db.GetCollection<Range>("demographics");

                return session.Query<Range>().ToList();
            }
        }

        public List<Event> GetEventStream(string patientId)
        {
            var eventStoreConnection = EventStoreConnection.Create(
                ConnectionSettings.Default,
                new IPEndPoint(IPAddress.Loopback, 1113));

            eventStoreConnection.ConnectAsync().Wait();

            var streamEventsSlice = eventStoreConnection
                .ReadStreamEventsForwardAsync("Encounter+" + patientId, 0,
                    4096, false)
                .GetAwaiter()
                .GetResult();
                
            return streamEventsSlice.Events
                .Select(e => new Event
                {
                    EventType = e.Event.EventType,
                    Created = e.Event.Created,
                    Data = JsonConvert.DeserializeObject(Encoding.UTF8.GetString(e.Event.Data), new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects })

                }).ToList();
        }
    }

    public class Event
    {
        public string EventType { get; set; }
        public DateTime Created { get; set; }
        public object Data { get; set; }
    }

    public class Range
    {
        public string Id { get; set; }

        public int Count { get; set; }
    }

    public class Patient
    {
        public string Id { get; set; }

        public int WardNumber { get; set; }

        public string PatientName { get; set; }

        public int AgeInYears { get; set; }
    }

    public class ProjectionState
    {
        public long CommitPosition { get; set; }

        public long PreparePosition { get; set; }
    }

    public class Ward
    {
        public int WardNumber { get; set; }

        public List<Patient> Patients { get; set; }
    }

    public class WardView
    {
        public List<Ward> Wards { get; set; }
    }
}