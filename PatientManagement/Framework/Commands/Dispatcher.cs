using System.Threading;
using System.Threading.Tasks;

namespace PatientManagement.Framework.Commands;

public class Dispatcher
{
    private readonly CommandHandlerMap _map;

    public Dispatcher(CommandHandlerMap map)
    {
        _map = map;
    }

    public Task Dispatch(object command, CancellationToken ct)
    {
        var handler = _map.Get(command);

        return handler(command, ct);
    }
}