using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventStore.Client;
using PatientManagement.Framework.Helpers;

namespace PatientManagement.Framework;

public class AggregateRepository
{
    readonly EventStoreClient _eventStore;

    public AggregateRepository(EventStoreClient eventStore)
    {
        _eventStore = eventStore;
    }

    public async Task<T> GetAsync<T>(Guid id, CancellationToken ct) where T : IAggregateRoot
    {
        var aggregateRoot = (T)Activator.CreateInstance(typeof(T), true)!;
        var events = await GetEvents(StreamName(typeof(T), id), ct);

        events.ForEach(aggregateRoot.Apply);
        aggregateRoot.ClearEvents();

        return aggregateRoot;
    }

    public Task SaveAsync(IAggregateRoot aggregateRoot, CancellationToken ct)
    {
        var events = aggregateRoot
            .GetEvents()
            .Select(ToEventData);

        return _eventStore.AppendToStreamAsync(
            StreamName(aggregateRoot.GetType(), aggregateRoot.Id), 
            StreamRevision.FromInt64(aggregateRoot.Version), 
            events,
            cancellationToken: ct
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

    async ValueTask<List<object>> GetEvents(string streamName, CancellationToken ct)
    {
        await using var readResult = _eventStore.ReadStreamAsync(
            Direction.Forwards,
            streamName,
            StreamPosition.Start,
            cancellationToken: ct
        );

        return await readResult
            .Select(@event => @event.Deserialize())
            .ToListAsync(ct);
    }
}