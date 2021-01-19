using System;

namespace ProjectionManager
{
    class EventHandler
    {
        public string EventType { get; init; }

        public Action<object> Handler { get; init; }
    }
}
