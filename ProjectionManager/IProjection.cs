using System.Threading;
using System.Threading.Tasks;

namespace ProjectionManager;

public interface IProjection
{
    bool CanHandle(string eventType);

    Task HandleAsync(string eventType, object e, CancellationToken ct);
}