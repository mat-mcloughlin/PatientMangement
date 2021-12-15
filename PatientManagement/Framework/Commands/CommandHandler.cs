using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PatientManagement.Framework.Commands;

public class CommandHandler
{
    internal Dictionary<string, Func<object, CancellationToken, Task>> Handlers { get; } = new();

    protected void Register<T>(Func<T, CancellationToken, Task> handler)
    {
        Handlers.Add(typeof(T).Name, (c, ct) => handler((T)c, ct));
    }
}