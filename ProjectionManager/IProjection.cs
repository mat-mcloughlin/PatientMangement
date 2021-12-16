namespace ProjectionManager;

public interface IProjection
{
    bool CanHandle(string eventType);

    void Handle(string eventType, object e);
}