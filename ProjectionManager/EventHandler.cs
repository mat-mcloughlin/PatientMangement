using System;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectionManager;

record EventHandler(
    string EventType,
    Func<object, CancellationToken, Task> Handler
);