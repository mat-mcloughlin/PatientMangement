using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectionManager;

public class Projection : IProjection
{
    readonly List<EventHandler> _handlers = new();

    protected void When<T>(Func<T, CancellationToken, Task> when)
    {
        _handlers.Add(new EventHandler(typeof(T).Name, (e, ct) => when((T)e, ct)));
    }

    async Task IProjection.HandleAsync(string eventType, object e, CancellationToken ct)
    {

        foreach (var handle in _handlers
                     .Where(h => h.EventType == eventType)
                     .Select(h => h.Handler(e, ct)))
        {
            await handle;
        }
    }

    bool IProjection.CanHandle(string eventType)
    {
        return _handlers.Any(h => h.EventType == eventType);
    }
}