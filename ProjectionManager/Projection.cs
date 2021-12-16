using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectionManager;

public class Projection : IProjection
{
    readonly List<EventHandler> _handlers = new();

    protected void When<T>(Action<T> when)
    {
        _handlers.Add(new EventHandler(typeof(T).Name, e => when((T)e)));
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