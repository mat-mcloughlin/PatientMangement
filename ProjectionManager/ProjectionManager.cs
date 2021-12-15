using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventStore.Client;
using PatientManagement.Framework.Helpers;

namespace ProjectionManager;

public class ProjectionManager
{
    readonly EventStoreClient _eventStore;

    readonly List<IProjection> _projections;

    readonly ConnectionFactory _connectionFactory;

    public ProjectionManager(
        EventStoreClient eventStore,
        ConnectionFactory connectionFactory,
        List<IProjection> projections)
    {
        _projections = projections;
        _eventStore = eventStore;
        _connectionFactory = connectionFactory;
    }

    public async Task StartAsync(CancellationToken ct)
    {
        foreach (var projection in _projections)
        {
            await StartProjectionAsync(projection, ct);
        }
    }

    async Task StartProjectionAsync(IProjection projection, CancellationToken ct)
    {
        var checkpoint = await GetPositionAsync(projection.GetType(), ct);

        if (checkpoint.HasValue)
        {
            await _eventStore.SubscribeToAllAsync(
                checkpoint.Value,
                EventAppeared(projection),
                false,
                ConnectionDropped(projection)!,
                cancellationToken: ct
            );
        }
        else
        {
            await _eventStore.SubscribeToAllAsync(
                EventAppeared(projection),
                false,
                ConnectionDropped(projection)!,
                cancellationToken: ct
            );
        }
    }

    private Action<StreamSubscription, SubscriptionDroppedReason, Exception> ConnectionDropped(
        IProjection projection)
    {
        return (_, reason, exc) =>
            Console.WriteLine(
                $"Projection {projection.GetType().Name} dropped with {reason}, Exception: {exc.Message} {exc.StackTrace}");
    }

    Func<StreamSubscription, ResolvedEvent, CancellationToken, Task> EventAppeared(IProjection projection)
    {
        return async (s, e, ct) =>
        {
            Console.WriteLine(
                $"Handling Projection {projection.GetType().Name} event {e.Event.EventType} id: {e.Event.EventId}");

            if (!projection.CanHandle(e.Event.EventType))
            {
                return ;
            }

            var deserializedEvent = e.Deserialize();
            await projection.HandleAsync(e.Event.EventType, deserializedEvent, ct);

            await UpdatePositionAsync(projection.GetType(), e.OriginalPosition!.Value, ct);
        };
    }

    async Task<Position?> GetPositionAsync(Type projection, CancellationToken ct)
    {
        using var session = _connectionFactory.Connect();
        var state = await session.LoadAsync<ProjectionState>(projection.Name, ct);

        if (state == null)
        {
            return null;
        }

        return new Position(state.CommitPosition, state.PreparePosition);
    }

    async Task UpdatePositionAsync(Type projection, Position position, CancellationToken ct)
    {
        using var session = _connectionFactory.Connect();
        var state = await session.LoadAsync<ProjectionState>(projection.Name, ct);

        if (state == null)
        {
            await session.StoreAsync(new ProjectionState
            {
                Id = projection.Name,
                CommitPosition = position.CommitPosition,
                PreparePosition = position.PreparePosition
            }, ct);
        }
        else
        {
            state.CommitPosition = position.CommitPosition;
            state.PreparePosition = position.PreparePosition;
        }

        await session.SaveChangesAsync(ct);
    }
}

public class ProjectionState
{
    public string Id { get; set; } = default!;

    public ulong CommitPosition { get; set; }

    public ulong PreparePosition { get; set; }
}