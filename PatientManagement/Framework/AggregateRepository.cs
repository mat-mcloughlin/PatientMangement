using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventStore.Client;
using PatientManagement.Framework.Helpers;
using EventData = EventStore.Client.EventData;

namespace PatientManagement.Framework;

public class AggregateRepository
{
    readonly EventStoreClient _eventStore;

    public AggregateRepository(EventStoreClient eventStore)
    {
        _eventStore = eventStore;
    }

    public async Task<T> Get<T>(Guid id) where T : IAggregateRoot
    {
        var aggregateRoot = (T)Activator.CreateInstance(typeof(T), true)!;
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

        return _eventStore.AppendToStreamAsync(
            StreamName(aggregateRoot.GetType(), aggregateRoot.Id), 
            StreamRevision.FromInt64(aggregateRoot.Version), 
            events
        );
    }

    static EventData ToEventData(object e)
    {
        return new EventData(
            Uuid.NewUuid(),
            e.GetType().Name,
            e.Serialize()
        );
    }

    static string StreamName(Type aggregate, Guid id)
    {
        return $"{aggregate.Name}-{id}";
    }

    async ValueTask<List<object>> GetEvents(string streamName)
    {
        await using var readResult = _eventStore.ReadStreamAsync(
            Direction.Forwards,
            streamName,
            StreamPosition.Start
        );

        return await readResult
            .Select(@event => @event.Deserialize())
            .ToListAsync();
    }
}