using System;

namespace ProjectionManager;

record EventHandler(
    string EventType,
    Action<object> Handler
);