using System;

namespace ProjectionManager
{
    class EventHandler
    {
        public string EventType { get; set; } = default!;

        public Action<object> Handler { get; set; } = default!;
    }
}