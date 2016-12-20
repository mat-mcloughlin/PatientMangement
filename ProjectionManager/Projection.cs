using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectionManager
{
    class Projection : IProjection
    {
        readonly List<EventHandler> _handlers = new List<EventHandler>();

        protected void When<T>(Action<T> when)
        {
            _handlers.Add(new EventHandler { EventType = typeof(T).Name, Handler = e => when((T)e) });
        }

        void IProjection.Handle(string eventType, object e)
        {
            _handlers
                .Where(h => h.EventType == eventType)
                .ToList()
                .ForEach(h => h.Handler(e));
        }

        bool IProjection.CanHandle(string eventType)
        {
            return _handlers.Any(h => h.EventType == eventType);
        }
    }
}