using System;

namespace ProjectionManager
{
    class EventHandler
    {
        public string EventType { get; set; }

        public Action<object> Handler { get; set; }
    }
}