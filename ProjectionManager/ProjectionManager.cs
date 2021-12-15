using System;
using System.Collections.Generic;
using EventStore.ClientAPI;
using PatientManagement.Framework.Helpers;

namespace ProjectionManager;

public class ProjectionManager
{
    readonly IEventStoreConnection _eventStoreConnection;

    readonly List<IProjection> _projections;

    readonly ConnectionFactory _connectionFactory;
    
    readonly CatchUpSubscriptionSettings defaultSettings = new(
        10000,
        500,
        false,
        false,
        string.Empty
    );

    public ProjectionManager(
        IEventStoreConnection eventStoreConnection,
        ConnectionFactory connectionFactory,
        List<IProjection> projections)
    {
        _projections = projections;
        _eventStoreConnection = eventStoreConnection;
        _connectionFactory = connectionFactory;
    }

    public void Start()
    {
        foreach (var projection in _projections)
        {
            StartProjection(projection);
        }
    }

    void StartProjection(IProjection projection)
    {
        var checkpoint = GetPosition(projection.GetType());
        
        _eventStoreConnection.SubscribeToAllFrom(
            checkpoint,
            defaultSettings,
            EventAppeared(projection),
            LiveProcessingStarted(projection),
            ConnectionDropped(projection));
    }

    private Action<EventStoreCatchUpSubscription, SubscriptionDropReason, Exception> ConnectionDropped(IProjection projection)
    {
        return (_, reason, exc) => Console.WriteLine($"Projection {projection.GetType().Name} dropped with {reason}, Exception: {exc.Message} {exc.StackTrace}");
    }

    Action<EventStoreCatchUpSubscription> LiveProcessingStarted(IProjection projection)
    {
        return s => Console.WriteLine($"Projection {projection.GetType().Name} has caught up, now processing live");
    }

    Action<EventStoreCatchUpSubscription, ResolvedEvent> EventAppeared(IProjection projection)
    {
        return (s, e) =>
        {
            Console.WriteLine(
                $"Handling Projection {projection.GetType().Name} event {e.Event.EventType} id: {e.Event.EventId}");
            
            if (!projection.CanHandle(e.Event.EventType))
            {
                return;
            }

            var deserializedEvent = e.Deserialize();
            projection.Handle(e.Event.EventType, deserializedEvent);

            UpdatePosition(projection.GetType(), e.OriginalPosition!.Value);
        };
    }

    Position? GetPosition(Type projection)
    {
        using var session = _connectionFactory.Connect();
        var state = session.Load<ProjectionState>(projection.Name);

        if (state == null)
        {
            return null;
        }

        return new Position(state.CommitPosition, state.PreparePosition);
    }

    void UpdatePosition(Type projection, Position position)
    {
        using var session = _connectionFactory.Connect();
        var state = session.Load<ProjectionState>(projection.Name);

        if (state == null)
        {
            session.Store(new ProjectionState
            {
                Id = projection.Name,
                CommitPosition = position.CommitPosition,
                PreparePosition = position.PreparePosition
            });
        }
        else
        {
            state.CommitPosition = position.CommitPosition;
            state.PreparePosition = position.PreparePosition;
        }

        session.SaveChanges();
    }
}

public class ProjectionState
{
    public string Id { get; set; } = default!;

    public long CommitPosition { get; set; }

    public long PreparePosition { get; set; }
}