using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using PatientManagement.Framework.Helpers;

namespace PatientManagement.Framework
{
    public class AggregateRepository
    {
        readonly IEventStoreConnection _connection;

        public AggregateRepository(IEventStoreConnection connection)
        {
            _connection = connection;
        }

        public async Task<T> Get<T>(Guid id) where T : IAggregateRoot
        {
            var aggregateRoot = (T)Activator.CreateInstance(typeof(T), true);
            var events = await GetEvents(StreamName(typeof(T), id));

            events.ForEach(aggregateRoot.Apply);
            aggregateRoot.ClearEvents();

            return aggregateRoot;
        }

        public Task Save(IAggregateRoot aggregateRoot)
        {
            var events = aggregateRoot
                .GetEvents()
                .Select(ToEventData);

            return _connection.AppendToStreamAsync(StreamName(aggregateRoot.GetType(), aggregateRoot.Id), aggregateRoot.Version, events);
        }

        static EventData ToEventData(object e)
        {
            return new EventData(
                Guid.NewGuid(),
                e.GetType().Name,
                true,
                e.Serialize(),
                null);
        }

        static string StreamName(Type aggregate, Guid id)
        {
            return $"{aggregate.Name}+{id}";
        }

        async Task<List<object>> GetEvents(string streamName)
        {
            var sliceStart = (long)StreamPosition.Start;
            var deserializedEvents = new List<object>();
            StreamEventsSlice slice;

            do
            {
                slice = await _connection.ReadStreamEventsForwardAsync(streamName, sliceStart, 200, false);
                deserializedEvents.AddRange(slice.Events.Select(e => e.Deserialize()));
                sliceStart = slice.NextEventNumber;

            } while (!slice.IsEndOfStream);

            return deserializedEvents;
        }
    }
}