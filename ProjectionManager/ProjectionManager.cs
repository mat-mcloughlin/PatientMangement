using System;
using System.Collections.Generic;
using EventStore.ClientAPI;
using MongoDB.Driver;
using PatientManagement.Framework.Helpers;

namespace ProjectionManager
{
    class ProjectionManager
    {
        readonly IEventStoreConnection _eventStoreConnection;

        readonly List<IProjection> _projections;

        readonly IMongoCollection<ProjectionState> _collection;

        public ProjectionManager(
            IEventStoreConnection eventStoreConnection,
            IMongoDatabase database,
            List<IProjection> projections)
        {
            _projections = projections;
            _eventStoreConnection = eventStoreConnection;
            _collection = database.GetCollection<ProjectionState>("ProjectionState");
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
                CatchUpSubscriptionSettings.Default,
                EventAppeared(projection),
                LiveProcessingStarted(projection));
        }

        Action<EventStoreCatchUpSubscription> LiveProcessingStarted(IProjection projection)
        {
            return s => Console.WriteLine($"Projection {projection.GetType().Name} has caught up, now processing live");
        }

        Action<EventStoreCatchUpSubscription, ResolvedEvent> EventAppeared(IProjection projection)
        {
            return (s, e) =>
            {
                if (!projection.CanHandle(e.Event.EventType))
                {
                    return;
                }

                var deserializedEvent = e.Deserialize();
                projection.Handle(e.Event.EventType, deserializedEvent);

                UpdatePosition(projection.GetType(), e.OriginalPosition.Value);
            };
        }

        Position? GetPosition(Type projection)
        {
            var filter = Builders<ProjectionState>.Filter.Eq(x => x.Id, projection.Name);

            var projectionState = _collection.Find(filter).FirstOrDefault();

            if (projectionState == null)
            {
                return null;
            }

            return new Position(projectionState.CommitPosition, projectionState.PreparePosition);
        }

        void UpdatePosition(Type projection, Position position)
        {
            var filter = Builders<ProjectionState>.Filter.Eq(x => x.Id, projection.Name);
            var update = Builders<ProjectionState>.Update.Set(x => x.CommitPosition, position.CommitPosition)
                .Set(x => x.PreparePosition, position.PreparePosition);
            var options = new UpdateOptions {IsUpsert = true};

            _collection.UpdateOne(filter, update, options);
        }
    }

    public class ProjectionState
    {
        public string Id { get; set; }

        public long CommitPosition { get; set; }

        public long PreparePosition { get; set; }
    }
}
